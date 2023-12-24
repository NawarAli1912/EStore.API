namespace Contracts.Products;

public record CreateProductResponse(
    Guid Id,
    string Name,
    string Description,
    int Quantity,
    decimal CustomerPrice);

public record CreateProductsResponse(
    List<CreateProductResponse> Items);

