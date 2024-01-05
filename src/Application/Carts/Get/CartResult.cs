using Domain.Customers.Enums;

namespace Application.Carts.Get;

public record CartResult(
    List<CartItemResult> Items,
    decimal TotalPrice);

public record CartItemResult(
    Guid ProductId,
    ItemType Type,
    int Quantity,
    decimal Price);