using Domain.Customers.Enums;
using Domain.Customers.ValueObjects;
using Domain.Errors;
using SharedKernel.Primitives;

namespace Domain.Customers.Entities;

public sealed class Cart : Entity
{
    private readonly HashSet<CartItem> _cartItems = [];

    public Guid CustomerId { get; private set; }

    public IReadOnlySet<CartItem> CartItems => _cartItems;

    internal static Cart Create(Customer customer)
    {
        return new Cart
        {
            CustomerId = customer.Id,
        };
    }

    private Cart() : base(Guid.NewGuid())
    {
    }

    internal Result<Updated> AddItem(CartItem item, ItemType type = ItemType.Product)
    {
        if (!_cartItems.TryGetValue(item,
                out var oldItem))
        {
            _cartItems.Add(item);

            return Result.Updated;
        }

        _cartItems.Remove(oldItem);

        var newItemResult = CartItem.Create(
            Id,
            item.ItemId,
            item.Quantity + oldItem.Quantity,
            type);

        if (newItemResult.IsError)
        {
            return newItemResult.Errors;
        }

        _cartItems.Add(newItemResult.Value);

        return Result.Updated;
    }

    internal Result<Updated> RemoveItem(CartItem item, ItemType type = ItemType.Product)
    {
        if (!_cartItems.TryGetValue(item, out var oldItem))
        {
            return DomainError.CartItems.NotFound;
        }

        _cartItems.Remove(oldItem);

        var newItemResult = CartItem
            .Create(Id, item.ItemId, oldItem.Quantity - item.Quantity, type);

        if (newItemResult.IsError)
        {
            return newItemResult.Errors;
        }

        if (newItemResult.Value.Quantity == 0)
        {
            return Result.Updated;
        }

        _cartItems.Add(newItemResult.Value);

        return Result.Updated;
    }

    internal void Clear()
    {
        _cartItems.Clear();
    }
}
