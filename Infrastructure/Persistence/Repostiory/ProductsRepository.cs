using Application.Common.Data;
using Application.Common.Repository;
using Application.Products.List;
using Dapper;
using Domain.Categories;
using Domain.ModelsSnapshots;
using Domain.Products;
using Nest;

namespace Infrastructure.Persistence.Repostiory;

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
        int rowsToSkip = (page - 1) * pageSize;

        await using var sqlConnection = _sqlConnectionFactory.Create();
        var products = await sqlConnection.QueryAsync<ProductSnapshot, CategorySnapshot, Product>(
                                $@"WITH ProductsWithRowNum AS (
                                SELECT 
                                    p.Id,
                                    p.Name, 
                                    p.Description, 
                                    p.Quantity,
                                    p.PurchasePrice,
                                    p.CustomerPrice,
                                    p.Sku,
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
                                    Sku,
                                    CategoryId,
                                    CategoryName,
                                    ParentCategoryId
                                FROM ProductsWithRowNum
                                WHERE ProductRank > {rowsToSkip} AND ProductRank <= {rowsToSkip + pageSize}",
                                (productSnap, categorySnap) =>
                                {
                                    var category =
                                            Category.Create(
                                            categorySnap.CategoryId,
                                            categorySnap.CategoryName,
                                            null!,
                                            parentCategoryId: categorySnap.ParentCategoryId);

                                    if (productDict.TryGetValue(productSnap.Id, out Product? product))
                                    {
                                        product.AssignCategory(category);

                                        return product;
                                    }

                                    product = Product.Create(
                                        productSnap.Id,
                                        productSnap.Name,
                                        productSnap.Description,
                                        productSnap.Quantity,
                                        productSnap.CustomerPrice,
                                        productSnap.PurchasePrice,
                                        productSnap.Sku).Value;

                                    product.AssignCategory(category);
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
                                @"SELECT
                                    COUNT(DISTINCT p.Id)
                                FROM
                                    Categories c 
                                LEFT JOIN 
                                    CategoryProduct cp ON c.Id = cp.CategoriesId
                                LEFT JOIN 
                                    Products p ON cp.ProductsId = p.Id
                                WHERE 
                                    c.Id IN @CategoryIds",
                                new { CategoryIds = categoriesIds.ToArray() });
    }

    public async Task<(List<Product>, int)> ListByFilter(ProductsFilter filter, int pageIndex, int pageSize)
    {
        var products = await _elasticClient
            .SearchAsync<ProductSnapshot>(s => s
            .Query(q =>
                q.Bool(b => b
                    .Must(must =>
                        !string.IsNullOrEmpty(filter.SearchTerm)
                            ? must.MultiMatch(m => m
                                .Query(filter.SearchTerm)
                                .Fuzziness(Fuzziness.Auto)
                                .Fields(f => f
                                    .Field(ff => ff.Name, boost: 2)
                                    .Field(ff => ff.Description)
                                )
                            )
                            : null
                    )
                    .Filter(f =>
                        f.Range(r => r
                                .Field(f => f.CustomerPrice)
                                .GreaterThanOrEquals((double?)filter.MinPrice ?? double.MinValue)
                            )
                        && f.Range(r => r
                                .Field(f => f.CustomerPrice)
                                .LessThanOrEquals((double?)filter.MaxPrice ?? double.MaxValue)
                            )
                        && f.Range(r => r
                                .Field(f => f.Quantity)
                                .GreaterThanOrEquals(filter.MinQuantity ?? int.MinValue)
                            )
                        &&
                           f.Range(r => r
                                .Field(f => f.Quantity)
                                .LessThanOrEquals(filter.MaxQuantity ?? int.MaxValue)
                            ))))
            .From((pageIndex - 1) * pageSize)
            .Size(pageSize));

        List<Product> result = [];
        foreach (var hit in products.Hits)
        {
            result.Add(Product.Create(
                hit.Source.Id,
                hit.Source.Name,
                hit.Source.Description,
                hit.Source.Quantity,
                hit.Source.CustomerPrice,
                hit.Source.PurchasePrice,
                hit.Source.Sku).Value);
        }

        return (result, (int)products.Total);
    }
}