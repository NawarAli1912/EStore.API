namespace Application.Products.Filters;

public record ListProductsDetailsFilter(
    string? SearchTerm,
    decimal? MinPrice,
    decimal? MaxPrice,
    int? MinQuantity,
    int? MaxQuantity
    );
