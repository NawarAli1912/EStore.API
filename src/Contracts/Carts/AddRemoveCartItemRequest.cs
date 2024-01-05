namespace Contracts.Carts;

public record AddRemoveCartItemRequest(
    Guid? ProductId,
    Guid? OfferId,
    int Quantity);

