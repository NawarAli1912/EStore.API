namespace Contracts.Carts;

public record AddCartItemRequest(
    Guid? ProductId,
    Guid? OfferId,
    int Quantity);

