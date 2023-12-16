using Domain.Customers.Entities;
using Domain.Customers.ValueObjects;
using SharedKernel;
using SharedKernel.Models;

namespace Domain.Customers;

public sealed class Customer : AggregateRoot<Guid>
{
    public Cart Cart { get; private set; } = default!;

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
}
