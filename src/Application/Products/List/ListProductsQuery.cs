using Application.Common.Cache;
using SharedKernel.Primitives;
using System.Text;

namespace Application.Products.List;

public record ListProductsQuery(
    ProductsFilter Filter,
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
            keyBuilder.Append($"SearchTerm:{Filter.SearchTerm ?? string.Empty};");
            keyBuilder.Append($"MinPrice:{Filter.MinPrice?.ToString() ?? string.Empty};");
            keyBuilder.Append($"MaxPrice:{Filter.MaxPrice?.ToString() ?? string.Empty};");
            keyBuilder.Append($"MinQuantity:{Filter.MinQuantity?.ToString() ?? string.Empty};");
            keyBuilder.Append($"MaxQuantity:{Filter.MaxQuantity?.ToString() ?? string.Empty};");
            keyBuilder.Append($"OnOffer:{Filter.OnOffer?.ToString() ?? string.Empty};");

            if (Filter.ProductStatus is not null && Filter.ProductStatus.Count != 0)
            {
                keyBuilder.Append("ProductStatus:");
                keyBuilder.Append(string.Join(',', Filter.ProductStatus));
                keyBuilder.Append(';');
            }

            keyBuilder.Append($"SortColumn:{Filter.SortColumn ?? "null"};");
            keyBuilder.Append($"SortOrder:{Filter.SortOrder ?? "null"};");
        }



        keyBuilder.Append($"Page:{Page};");
        keyBuilder.Append($"PageSize:{PageSize};");

        return keyBuilder.ToString();
    }

}
