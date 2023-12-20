namespace Contracts.Products;

public record ListProductsFilter(
    string? SearchTerm,
    decimal? MinPrice,
    decimal? MaxPrice);
