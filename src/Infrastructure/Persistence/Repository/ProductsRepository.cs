using Application.Common.DatabaseAbstraction;
using Application.Common.Repository;
using Application.Products.List;
using Dapper;
using Domain.Categories;
using Domain.ModelsSnapshots;
using Domain.Products;
using Nest;

namespace Infrastructure.Persistence.Repository;

public sealed class ProductsRepository(
        ISqlConnectionFactory sqlConnectionFactory,
        IElasticClient elasticClient,
        ApplicationDbContext context)
    : IProductsRepository
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;
    private readonly IElasticClient _elasticClient = elasticClient;
    private readonly ApplicationDbContext _context = context;

    public async Task<List<Product>> ListByCategories(
        IEnumerable<Guid> categoryIds,
        int page,
        int pageSize)
    {
        var productDict = new Dictionary<Guid, Product>();
        var rowsToSkip = (page - 1) * pageSize;
        await using var sqlConnection = _sqlConnectionFactory.Create();
        var products = await sqlConnection.QueryAsync<ProductSnapshot, CategorySnapshot, Product>(
            $"""
             WITH ProductsWithRowNum AS (
                 SELECT
                     p.Id,
                     p.Name,
                     p.Description,
                     p.Quantity,
                     p.PurchasePrice,
                     p.CustomerPrice,
                     c.Id AS CategoryId,
                     c.Name AS CategoryName,
                     c.ParentCategoryId,
             	    DENSE_RANK() OVER (ORDER BY p.Id) AS ProductRank
                 FROM
                     Products p
                 JOIN
                     CategoryProduct cp ON p.Id = cp.ProductsId
                 JOIN
                     Categories c ON cp.CategoriesId = c.Id
                 WHERE
                     cp.CategoriesId IN @CategoryIds)
                 SELECT
                     Id,
                     Name,
                     Description,
                     Quantity,
                     PurchasePrice,
                     CustomerPrice,
                     CategoryId,
                     CategoryName,
                     ParentCategoryId
                 FROM ProductsWithRowNum
                 WHERE 
                    ProductRank > {rowsToSkip} AND ProductRank <= {rowsToSkip + pageSize}
             """,
        (productSnap, categorySnap) =>
            {
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
                    productSnap.PurchasePrice);

                product.AssignCategories([category]);
                product.AssociateOffers([.. productSnap.AssociatedOffers]);
                productDict.Add(productSnap.Id, product);
                return product;
            },
        new
        {
            CategoryIds = categoryIds.ToArray()
        },
            splitOn: "CategoryId");

        return [.. productDict.Values];
    }

    public async Task<int> GetProductCountByCategory(IEnumerable<Guid> categoriesIds)
    {
        await using var sqlConnection = _sqlConnectionFactory.Create();
        return await sqlConnection
                            .ExecuteScalarAsync<int>(
                                """
                                SELECT
                                    COUNT(DISTINCT p.Id)
                                FROM
                                    Categories c
                                LEFT JOIN
                                    CategoryProduct cp ON c.Id = cp.CategoriesId
                                LEFT JOIN
                                    Products p ON cp.ProductsId = p.Id
                                WHERE
                                    c.Id IN @CategoryIds
                                """,
                                new { CategoryIds = categoriesIds.ToArray() });
    }

    public async Task<(List<Product>, int)> ListByFilter(ProductsFilter filter, int pageIndex, int pageSize)
    {
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
}