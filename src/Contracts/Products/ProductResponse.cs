namespace Contracts.Products;

public record ProductDetailedResponse(
    Guid Id,
    string Name,
    string Description,
    int Quantity,
    decimal CustomerPrice,
    decimal PurchasePrice,
    List<CategoryResponse> Categories,
    List<Guid> AssociatedOffers);

public record ProductResponse(
    Guid Id,
    string Name,
    string Description,
    int Quantity,
    decimal CustomerPrice,
    List<Guid> AssociatedOffers);

public record CategoryResponse(
    Guid Id,
    string Name);
