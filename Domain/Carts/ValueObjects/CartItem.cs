using Domain.Kernal.Models;

namespace Domain.Carts.ValueObjects;

public class CartItem : ValueObject
{
    private CartItem(Guid cartId, Guid productId, int quantity)
    {
        CartId = cartId;
        ProductId = productId;
        Quantity = quantity;
    }

    public Guid CartId { get; init; }

    public Guid ProductId { get; init; }

    public int Quantity { get; init; }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return CartId;
        yield return ProductId;
        yield return Quantity;
    }

    public static CartItem Create(Guid cartId, Guid productId, int quantity)
    {
        return new(cartId, productId, quantity);
    }
}
