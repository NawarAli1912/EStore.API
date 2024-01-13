using Application.Common.DatabaseAbstraction;
using Application.Common.Repository;
using Application.Products.List;
using Dapper;
using Domain.Categories;
using Domain.ModelsSnapshots;
using Domain.Products;
using Nest;
using Newtonsoft.Json;
using Serilog;

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

        Dictionary<Guid, Product> productDict = [];
        foreach (var product in products)
        {
            if (productDict.TryGetValue(product.Id, out var dictProduct))
            {
                dictProduct.AssignCategories(product.Categories);
                continue;
            }

            productDict.Add(product.Id, product);
        }

        var totalProducts = multiQueryResult
            .Read<int>().Single();

        return (productDict.Values.ToList(), totalProducts);
    }

    public async Task<(List<Product>, int)> ListByFilter(
        ProductsFilter filter,
        int pageIndex,
        int pageSize)
    {
        var dbPrdocuts = FallBackToDbQuery(filter, pageIndex, pageSize);
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

        var sortingField = filter.SortColumn ?? "_score";
        ISearchResponse<ProductSnapshot> searchResponse;
        try
        {
            searchResponse = await _elasticClient.SearchAsync<ProductSnapshot>(s => s
                .Query(_ => query)
                .Sort(s =>
                    s.Field(f =>
                    {
                        f.Order(filter.SortOrder == "asc"
                                ? SortOrder.Ascending
                                : SortOrder.Descending);

                        switch (sortingField)
                        {
                            case "name":
                                f.Field(ff => ff.Name);
                                break;
                            case "price":
                                f.Field(ff => ff.CustomerPrice);
                                break;
                            case "quantity":
                                f.Field(ff => ff.Quantity);
                                break;
                            default:
                                f.Field("_score");
                                f.Descending();
                                break;
                        }
                        return f;
                    }))
                .From((pageIndex - 1) * pageSize)
                .Size(pageSize));
        }
        catch (Exception ex)
        {
            Log.Error("elastic search query doesn't work with exception {ex}", ex);

            return await dbPrdocuts;
        }

        return ProcessElasticsearchResponse(searchResponse);
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
                    p.AssociatedOffers)));

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

        var products = multiQueryResult.Read<ProductSnapshot, CategorySnapshot, Product>(
            MapProductCategorySanpShotToProduct,
            splitOn: "CategoryId");

        Dictionary<Guid, Product> productDict = [];
        foreach (var product in products)
        {
            if (productDict.TryGetValue(product.Id, out var dictProduct))
            {
                dictProduct.AssignCategories(product.Categories);
                continue;
            }

            productDict.Add(product.Id, product);
        }

        var totalProducts = multiQueryResult
                .Read<int>()
                .Single();

        return (productDict.Values.ToList(), totalProducts);
    }

    private Product MapProductCategorySanpShotToProduct(
        ProductSnapshot productSnap,
        CategorySnapshot categorySnap)
    {
        var category =
            Category.Create(
            categorySnap.CategoryId,
            categorySnap.CategoryName,
            categorySnap.ParentCategoryId);

        var product = Product.Create(
            productSnap.Id,
            productSnap.Name,
            productSnap.Description,
            productSnap.Quantity,
            productSnap.CustomerPrice,
            productSnap.PurchasePrice,
            productSnap.Code);

        product.AssignCategories([category]);
        product.AssociateOffers(
            JsonConvert.DeserializeObject<List<Guid>>(productSnap.AssociatedOffersString) ?? []);

        return product;
    }
}