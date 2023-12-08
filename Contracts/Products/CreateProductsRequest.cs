namespace Contracts.Products;

public sealed record CreateProductsRequest(List<CreateProductRequest> Items);

public sealed record CreateProductRequest(
    string Name,
    string Description,
    int Quantity,
    decimal PurchasePrice,
    decimal CustomerPrice,
    string Currency,
    string? Sku,
    List<Guid>? Categories);