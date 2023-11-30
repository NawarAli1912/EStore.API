namespace Contracts.Products;

public sealed record CreateProductRequest(
    string Name,
    string Description,
    int Quantity,
    decimal Price,
    string Currency,
    string? Sku);
