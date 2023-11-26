namespace Domain.Carts.ValueObjects;

public record CartItem(Guid CartId, Guid ProductId, int Quantity);
