namespace Contracts.Products;

public record CreateProductResponse(
    Guid Id,
    string Name,
    string Description,
    int Quantity,
    decimal CustomerPrice,
    string Sku);

public record CreateProductsResponse(
    List<CreateProductResponse> Items);

