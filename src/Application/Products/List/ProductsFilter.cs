using Domain.Products.Enums;

namespace Application.Products.List;

public record ProductsFilter(
    string? SearchTerm,
    decimal? MinPrice,
    decimal? MaxPrice,
    int? MinQuantity,
    int? MaxQuantity,
    bool? OnOffer,
    List<ProductStatus> ProductStatus,
    string? SortColumn,
    string? SortOrder);
