using SharedKernel.Enums;

namespace Application.Carts.Get;

public record CartResult(
    List<CartItemResult> Items,
    decimal TotalPrice);

public record CartItemResult(
    Guid ItemId,
    ItemType Type,
    int Quantity,
    decimal Price);