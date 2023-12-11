namespace Contracts.Products;

public record ProductDetailedResponse(
    Guid Id,
    string Name,
    string Description,
    int Quantity,
    decimal CustomerPrice,
    decimal PurchasePrice,
    string Sku,
    List<CategoryResponse> Categories
    );

public record ProductResponse(
    Guid Id,
    string Name,
    string Description,
    decimal CustomerPrice);

public record CategoryResponse(
    Guid Id,
    string Name);
