using Domain.Customers.ValueObjects;
using Domain.Kernal.Models;

namespace Domain.Customers.Entities;

public sealed class Cart : Entity<Guid>
{
    private readonly HashSet<CartItem> _cartItems = [];

    public Guid CustomerId { get; set; }

    public IReadOnlySet<CartItem> CartItems => _cartItems.ToHashSet();

    public static Cart Create(Customer customer)
    {
        return new Cart
        {
            CustomerId = customer.Id,
        };
    }

    private Cart() : base(Guid.NewGuid())
    {
    }
}
