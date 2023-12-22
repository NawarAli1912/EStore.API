using Domain.Customers.Errors;
using SharedKernel.Models;
using SharedKernel.Primitives;

namespace Domain.Customers.ValueObjects;

public sealed class CartItem : ValueObject
{
    public Guid CartId { get; init; }

    public Guid ProductId { get; init; }

    public int Quantity { get; init; }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return ProductId;
    }

    internal static Result<CartItem> Create(
        Guid cartId,
        Guid productId,
        int quantity)
    {
        if (quantity < 0)
        {
            return DomainError.CartItem.NegativeQuantity;
        }

        return new CartItem
        {
            CartId = cartId,
            ProductId = productId,
            Quantity = quantity
        };
    }
}
