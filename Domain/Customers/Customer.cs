using Domain.Customers.Entities;
using Domain.Kernal.Models;

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
}
