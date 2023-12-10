namespace Contracts.Carts;
public record RemoveFromCartRequest(
    Guid ProductId,
    int Quantity);
