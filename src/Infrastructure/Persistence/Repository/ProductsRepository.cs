using Application.Common.DatabaseAbstraction;
using Application.Common.Repository;
using Application.Products.List;
using Dapper;
using Domain.Categories;
using Domain.ModelsSnapshots;
using Domain.Products;
using Nest;
using Newtonsoft.Json;

namespace Infrastructure.Persistence.Repository;

public sealed class ProductsRepository(
        ISqlConnectionFactory sqlConnectionFactory,
        IElasticClient elasticClient)
    : IProductsRepository
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
    private readonly IElasticClient _elasticClient = elasticClient;

    public async Task<(List<Product>, int)> ListByCategories(
        IEnumerable<Guid> categoryIds,
        int pageIndex,
        int pageSize)
    {
        var productDict = new Dictionary<Guid, Product>();
        await using var sqlConnection = _sqlConnectionFactory.Create();

        var multiQueryResult =
            await sqlConnection.QueryMultipleAsync(
                    $"{SqlQueries.ProductsCategoryFilter}; {SqlQueries.ProductsCategoryCount};",
                    new
                    {
                        CategoryIds = categoryIds.ToArray(),
                        PageIndex = pageIndex,
                        PageSize = pageSize
                    });

        var products = multiQueryResult
            .Read<ProductSnapshot, CategorySnapshot, Product>(MapProductCategorySanpShotToProduct, splitOn: "CategoryId");

        var totalProducts = multiQueryResult
            .Read<int>().Single();

        return ([.. products], totalProducts);
    }

    public async Task<(List<Product>, int)> ListByFilter(
        ProductsFilter filter,
        int pageIndex,
        int pageSize)
    {
        if (!await CheckConnection())
        {
            return await FallBackToDbQuery(filter, pageIndex, pageSize);
        }

        var baseQuery = new QueryContainerDescriptor<ProductSnapshot>();
        var searchTermQuery = !string.IsNullOrEmpty(filter.SearchTerm) ?
                baseQuery.MultiMatch(m => m
                    .Query(filter.SearchTerm)
                    .Fuzziness(Fuzziness.Auto)
                    .Fields(fs => fs
                        .Field(f => f.Name, boost: 2)
                        .Field(f => f.Description)))
                : null;

        var priceRangeQuery = baseQuery.Range(r => r
            .Field(f => f.CustomerPrice)
            .GreaterThanOrEquals((double?)filter.MinPrice ?? double.MinValue)
            .LessThanOrEquals((double?)filter.MaxPrice ?? double.MaxValue));


        var quantityRangeQuery = baseQuery.Range(r => r
            .Field(f => f.Quantity)
            .GreaterThanOrEquals(filter.MinQuantity ?? int.MinValue)
            .LessThanOrEquals(filter.MaxQuantity ?? int.MaxValue));

        var statusQuery = baseQuery.Terms(t => t
            .Field(f => f.Status)
            .Terms(filter.ProductStatus));

        QueryContainer? offersQuery = null;
        if (filter.OnOffer.HasValue)
        {
            offersQuery = filter.OnOffer.Value ?
                baseQuery.Exists(e => e.Field(f => f.AssociatedOffers)) :
                !baseQuery.Exists(e => e.Field(f => f.AssociatedOffers));
        }

        var query = baseQuery.Bool(b => b
            .Must(searchTermQuery)
            .Filter(priceRangeQuery, quantityRangeQuery, statusQuery, offersQuery));

        var searchResponse = await _elasticClient.SearchAsync<ProductSnapshot>(s => s
                .Query(_ => query)
                .From((pageIndex - 1) * pageSize)
                .Size(pageSize));

        return ProcessElasticsearchResponse(searchResponse); ;
    }

    public async Task<int> GetProductCountByCategory(IEnumerable<Guid> categoriesIds)
    {
        await using var sqlConnection = _sqlConnectionFactory.Create();
        return await sqlConnection
                            .ExecuteScalarAsync<int>(
                                SqlQueries.ProductsCategoryCount,
                                new { CategoryIds = categoriesIds.ToArray() });
    }

    private (List<Product>, int) ProcessElasticsearchResponse(
        ISearchResponse<ProductSnapshot> searchResponse)
    {
        var products = searchResponse.Documents.ToList();
        var categoriesDict = new Dictionary<Guid, List<Category>>();

        foreach (var product in products)
        {
            foreach (var categorySnapshot in product.Categories)
            {
                var category = Category.Create(
                    categorySnapshot.CategoryId,
                    categorySnapshot.CategoryName,
                    parentCategoryId: categorySnapshot.ParentCategoryId);

                if (categoriesDict.TryGetValue(product.Id, out var cateogires))
                {
                    categoriesDict[product.Id].Add(category);
                    continue;
                }

                categoriesDict[product.Id] = [category];
            }
        }

        List<Product> result = [];
        result.AddRange(products
                .Select(p => Product.Create(
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Quantity,
                    p.CustomerPrice,
                    p.PurchasePrice,
                    p.Code,
                    categoriesDict.GetValueOrDefault(p.Id),
                    JsonConvert.DeserializeObject<List<Guid>>(p.AssociatedOffers))));

        return (result, (int)searchResponse.Total);
    }

    private async Task<(List<Product>, int)> FallBackToDbQuery(
        ProductsFilter filter,
        int pageIndex,
        int pageSize)
    {
        await using var sqlConnection = _sqlConnectionFactory.Create();
        var multiQueryResult = await sqlConnection.QueryMultipleAsync(
            $"{SqlQueries.ProductsFilterQuery}; {SqlQueries.ProductsFilterCount};",
             new
             {
                 filter.SearchTerm,
                 filter.MinPrice,
                 filter.MaxPrice,
                 filter.MinQuantity,
                 filter.MaxQuantity,
                 Stasuses = filter.ProductStatus,
                 PageIndex = pageIndex,
                 PageSize = pageSize,
                 filter.OnOffer
             });

        var products = multiQueryResult
            .Read<ProductSnapshot, CategorySnapshot, Product>(MapProductCategorySanpShotToProduct, splitOn: "CategoryId");

        var totalProducts = multiQueryResult.Read<int>().Single();
        return (products.ToList(), totalProducts);
    }

    private Product MapProductCategorySanpShotToProduct(
        ProductSnapshot productSnap,
        CategorySnapshot categorySnap)
    {
        var productDict = new Dictionary<Guid, Product>();
        var category =
            Category.Create(
            categorySnap.CategoryId,
            categorySnap.CategoryName,
            categorySnap.ParentCategoryId);

        if (productDict.TryGetValue(productSnap.Id, out Product? product))
        {
            product.AssignCategories([category]);

            return product;
        }

        product = Product.Create(
            productSnap.Id,
            productSnap.Name,
            productSnap.Description,
            productSnap.Quantity,
            productSnap.CustomerPrice,
            productSnap.PurchasePrice,
            productSnap.Code);

        product.AssignCategories([category]);
        product.AssociateOffers(
            JsonConvert.DeserializeObject<List<Guid>>(productSnap.AssociatedOffers)
            ?? []);

        productDict.Add(productSnap.Id, product);
        return product;
    }

    private async Task<bool> CheckConnection()
    {
        try
        {
            var q = await _elasticClient.PingAsync();
            if (q.IsValid)
                return true;

        }
        catch
        {
            return false;
        }

        return false;
    }
}