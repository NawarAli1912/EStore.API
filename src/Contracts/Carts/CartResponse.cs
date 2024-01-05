namespace Contracts.Carts;
public record CartResponse(
    List<CartItemResponse> Items,
    decimal TotalPrice);

public record CartItemResponse(
    Guid ProductId,
    ItemType type,
    int Quantity,
    decimal Price);