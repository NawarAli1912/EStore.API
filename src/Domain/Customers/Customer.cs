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

    public Result<Updated> AddCartItem(Guid productId, int quantity)
    {
        var cartItem = CartItem.Create(Cart.Id, productId, quantity);

        if (cartItem.IsError)
        {
            return cartItem.Errors;
        }

        var addResult = Cart.AddItem(cartItem.Value);

        if (addResult.IsError)
        {
            return addResult.Errors;
        }

        return addResult.Value;
    }

    public Result<Updated> RemoveCartItem(Guid productId, int quantity)
    {
        var cartItemResult = CartItem.Create(Cart.Id, productId, quantity);

        if (cartItemResult.IsError)
        {
            return cartItemResult.Errors;
        }

        var removeResult = Cart.RemoveItem(cartItemResult.Value);

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
