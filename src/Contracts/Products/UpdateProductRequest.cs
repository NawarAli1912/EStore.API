namespace Contracts.Products;
public record UpdateProductRequest(
    string? Name,
    string? Description,
    int? Quantity,
    decimal? PurchasePrice,
    decimal? CustomerPrice);
