namespace Application.Carts.Get;

public record CartResult(
    List<CartItemResult> Items,
    decimal TotalPrice);

public record CartItemResult(
    Guid ProductId,
    int Quantity,
    decimal Price);