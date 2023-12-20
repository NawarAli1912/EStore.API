using Application.Common.Cache;
using SharedKernel;
using System.Text;

namespace Application.Products.List;

public record ListProductsQuery(
    ProductsFilter Filter,
    string? SortColumn,
    string? SortOrder,
    int Page = 1,
    int PageSize = 10) : ICachedQuery<Result<ListProductResult>>
{
    public string Key => ConstructCacheKey();

    public TimeSpan? Expiration => null;

    private string ConstructCacheKey()
    {
        var keyBuilder = new StringBuilder();

        if (Filter != null)
        {
            keyBuilder.Append($"SearchTerm:{Filter.SearchTerm ?? "null"};");
            keyBuilder.Append($"MinPrice:{Filter.MinPrice ?? 0};");
            keyBuilder.Append($"MaxPrice:{Filter.MaxPrice ?? 0};");
            keyBuilder.Append($"MinQuantity:{Filter.MinQuantity ?? 0};");
            keyBuilder.Append($"MaxQuantity:{Filter.MaxQuantity ?? 0};");

            if (Filter.ProductStatus != null && Filter.ProductStatus.Any())
            {
                keyBuilder.Append("ProductStatus:");
                keyBuilder.Append(string.Join(',', Filter.ProductStatus));
                keyBuilder.Append(";");
            }
        }

        keyBuilder.Append($"SortColumn:{SortColumn ?? "null"};");
        keyBuilder.Append($"SortOrder:{SortOrder ?? "null"};");

        keyBuilder.Append($"Page:{Page};");
        keyBuilder.Append($"PageSize:{PageSize};");

        return keyBuilder.ToString();
    }
}
