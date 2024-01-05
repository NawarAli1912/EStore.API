using Domain.Customers.Entities;
using Domain.Customers.Enums;
using Domain.Customers.ValueObjects;
using SharedKernel.Primitives;

namespace Domain.Customers;

public sealed class Customer : AggregateRoot
{
    public Cart Cart { get; private set; } = default!;

    public CustomerStatus Status { get; private set; } = CustomerStatus.Active;

    private Customer(Guid id) : base(id)
    {
    }

    public static Customer Create(Guid id)
    {
        var customer = new Customer(id);

        var cart = Cart.Create(customer);
        customer.Cart = cart;

        return customer;
    }

    public Result<Updated> AddCartItem(Guid itemId, int quantity, ItemType type = ItemType.Product)
    {
        var cartItem = CartItem
            .Create(Cart.Id, itemId, quantity, type);

        if (cartItem.IsError)
        {
            return cartItem.Errors;
        }

        var addResult = Cart.AddItem(cartItem.Value, type);

        if (addResult.IsError)
        {
            return addResult.Errors;
        }

        return addResult.Value;
    }

    public Result<Updated> RemoveCartItem(Guid itemId, int quantity, ItemType type = ItemType.Product)
    {
        var cartItemResult = CartItem.Create(Cart.Id, itemId, quantity, type);

        if (cartItemResult.IsError)
        {
            return cartItemResult.Errors;
        }

        var removeResult = Cart.RemoveItem(cartItemResult.Value, type);

        if (removeResult.IsError)
        {
            return removeResult.Errors;
        }

        return removeResult.Value;
    }

    public void ClearCart()
    {
        Cart.Clear();
    }

    private Customer() : base(Guid.NewGuid())
    {
    }
}
