using Domain.Customers.Entities;
using Domain.Customers.ValueObjects;
using Domain.Kernal.Models;

namespace Domain.Customers;

public sealed class Customer : AggregateRoot<Guid>
{
    public Cart Cart { get; private set; } = default!;

    public Address Address { get; private set; } = default!;

    private Customer(Guid id) : base(id)
    {
    }

    public static Customer Create(Guid id, Address address)
    {
        var customer = new Customer(id)
        {
            Address = address
        };

        var cart = Cart.Create(customer);
        customer.Cart = cart;

        return customer;
    }
}
