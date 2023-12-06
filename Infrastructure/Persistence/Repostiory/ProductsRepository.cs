using Application.Common.Data;
using Application.Repository;
using Dapper;
using Domain.Categories;
using Domain.Kernal;
using Domain.ModelsSnapshots;
using Domain.Products;

namespace Infrastructure.Persistence.Repostiory;

public sealed class ProductsRepository(ISqlConnectionFactory sqlConnectionFactory) : IProductsRepository
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;

    public async Task<List<Product>> GetByCategories(IEnumerable<Guid> categoryIds)
    {
        var productDict = new Dictionary<Guid, Product>();

        await using var sqlConnection = _sqlConnectionFactory.Create();
        var products = await sqlConnection.QueryAsync<ProductSnapshot, CategorySnapshot, Product>(
                                @"SELECT 
                                    DISTINCT	                                                         
                                    p.Id,
                                    p.Name, 
                                    p.Description, 
                                    p.Quantity,
                                    p.PurchasePrice_Value,
                                    p.PurchasePrice_Currency,
                                    p.CustomerPrice_Value,
                                    p.Sku,
                                    c.Id AS CategoryId,
                                    c.Name AS CategoryName,
                                    c.ParentCategoryId
                                FROM 
                                    Products p 
                                JOIN 
                                    CategoryProduct cp ON p.Id = cp.ProductsId
                                JOIN
                                    Categories c ON cp.CategoriesId = c.Id
                                WHERE 
                                    cp.CategoriesId IN @CategoryIds",
                                (productSnap, categorySnap) =>
                                {
                                    var category =
                                            Category.Create(
                                            categorySnap.CategoryId,
                                            categorySnap.CategoryName,
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
                                        productSnap.CustomerPrice_Value,
                                        productSnap.PurchasePrice_Value,
                                        Enum.GetName(typeof(Currency), productSnap.CustomerPrice_Currency)!,
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
}
