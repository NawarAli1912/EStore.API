namespace Contracts.Carts;

public record RemoveCartItemRequest(Guid ItemId, int Quantity);

