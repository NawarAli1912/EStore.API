namespace Contracts.Products;

public record ListProductsDetailsFilter(
    string? SearchTerm,
    decimal? MinPrice,
    decimal? MaxPrice,
    int? MinQuantity,
    int? MaxQuantity,
    IEnumerable<ProductStatus>? Status
    );
