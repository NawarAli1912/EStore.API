namespace Contracts.Products;

public record CreateProductResponse(
    Guid Id,
    string Name,
    string Description,
    int Quantity,
    decimal Price,
    string Currency,
    string Sku);

