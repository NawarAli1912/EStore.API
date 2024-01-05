using Domain.Errors;
using SharedKernel.Enums;
using SharedKernel.Primitives;

namespace Domain.Customers.ValueObjects;

public sealed class CartItem : ValueObject
{
    public Guid CartId { get; init; }

    public Guid ItemId { get; init; }

    public int Quantity { get; init; }

    public ItemType Type { get; init; } = ItemType.Product;

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return ItemId;
    }

    internal static Result<CartItem> Create(
        Guid id,
        Guid ItemId,
        int quantity,
        ItemType type = ItemType.Product)
    {
        if (quantity < 0)
        {
            return DomainError.CartItems.NegativeQuantity;
        }

        return new CartItem
        {
            CartId = id,
            ItemId = ItemId,
            Quantity = quantity,
            Type = type
        };
    }
}
