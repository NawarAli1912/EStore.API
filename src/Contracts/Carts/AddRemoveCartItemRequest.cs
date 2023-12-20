namespace Contracts.Carts;

public record AddRemoveCartItemRequest(
    Guid ProductId,
    int Quantity);

