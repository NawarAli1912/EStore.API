using Application.Common.DatabaseAbstraction;
using Application.Common.Repository;
using Application.Products.List;
using Dapper;
using Domain.Categories;
using Domain.ModelsSnapshots;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
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

        (List<Product>, int) result;
        try
        {
            var searchResponse = await _elasticClient.SearchAsync<ProductSnapshot>(s => s
                    .Query(_ => query)
                    .From((pageIndex - 1) * pageSize)
                    .Size(pageSize));

            result = ProcessElasticsearchResponse(searchResponse);

        }
        catch
        {
            result = await FallBackToDbQuery(filter, pageIndex, pageSize);
        }

        return result;
    }

    private (List<Product>, int) ProcessElasticsearchResponse(ISearchResponse<ProductSnapshot> searchResponse)
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

    private async Task<(List<Product>, int)> FallBackToDbQuery(ProductsFilter filter, int pageIndex, int pageSize)
    {

        /*var baseQuery = @"
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
		                    (@SearchTerm IS NULL OR P.Name LIKE '%' + @SearchTerm + '%' OR P.Description LIKE '%' + @SearchTerm + '%')
		                    AND (@MinPrice IS NULL OR P.CustomerPrice >= @MinPrice)
		                    AND (@MaxPrice IS NULL OR P.CustomerPrice <= @MaxPrice)
		                    AND (@MinQuantity IS NULL OR P.Quantity >= @MinQuantity)
		                    AND (@MaxQuantity IS NULL OR P.Quantity <= @MaxQuantity)
		                    AND (
			                    @OnOffer IS NULL
			                    OR (
				                    @OnOffer = 1 AND P.AssociatedOffers IS NOT NULL AND JSON_QUERY(P.AssociatedOffers) <> '[]'
			                    )
			                    OR (
				                    @OnOffer = 0 AND (P.AssociatedOffers IS NULL OR JSON_QUERY(P.AssociatedOffers) = '[]')
			                    )
		                    ))
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
                        FROM 
	                        ProductsWithRowNum
                        WHERE 
                            ProductRank > (@PageIndex-1)*@PageIndex AND ProductRank <= ((@PageIndex-1)*@PageIndex) + @PageSize
                        ORDER BY 
                            Id
                        ";*/
        var query = _context
            .Products
            .Include(p => p.Categories)
            .AsQueryable();

        if (!string.IsNullOrEmpty(filter.SearchTerm))
        {
            query = query.Where(p => p.Name.Contains(filter.SearchTerm) || p.Description.Contains(filter.SearchTerm));
        }

        if (filter.MinPrice.HasValue)
        {
            query = query.Where(p => p.CustomerPrice >= filter.MinPrice.Value);
        }

        if (filter.MaxPrice.HasValue)
        {
            query = query.Where(p => p.CustomerPrice <= filter.MaxPrice.Value);
        }

        if (filter.MinQuantity.HasValue)
        {
            query = query.Where(p => p.Quantity >= filter.MinQuantity.Value);
        }

        if (filter.MaxQuantity.HasValue)
        {
            query = query.Where(p => p.Quantity <= filter.MaxQuantity.Value);
        }

        if (filter.ProductStatus != null && filter.ProductStatus.Any())
        {
            query = query.Where(p => filter.ProductStatus.Contains(p.Status));
        }

        if (filter.OnOffer.HasValue)
        {
            if (filter.OnOffer.Value)
            {
                query = query
                    .Where(p => p.AssociatedOffers.Any());
            }
            else
            {
                query = query
                    .Where(p => !p.AssociatedOffers.Any());
            }
        }


        var totalProducts = await query.CountAsync();
        var products = await query
            .OrderBy(p => p.Id)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var productsWithOffers = products.Where(p => p.AssociatedOffers.Any()).ToList();

        return (products, totalProducts);
    }
}