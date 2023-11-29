using Domain.Customers.ValueObjects;
using Domain.Kernal.Models;

namespace Domain.Customers.Entities;

public class Cart : Entity<Guid>
{
    private readonly HashSet<CartItem> _cartItems = new();

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
