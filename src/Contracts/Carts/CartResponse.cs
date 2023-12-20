namespace Contracts.Carts;
public record CartResponse(
    List<CartItemResponse> Items,
    decimal TotalPrice);

public record CartItemResponse(
    Guid ProductId,
    int Quantity,
    decimal Price);