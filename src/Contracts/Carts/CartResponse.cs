namespace Contracts.Carts;
public record CartResponse(
    List<CartItemResponse> Items,
    decimal TotalPrice);

public record CartItemResponse(
    Guid ItemId,
    ItemType Type,
    int Quantity,
    decimal Price);