namespace Application.Products.Filters;

public record ListProductFilter(
    string? SearchTerm,
    decimal? MinPrice,
    decimal? MaxPrice,
    int? MinQuantity,
    int? MaxQuantity
    );
