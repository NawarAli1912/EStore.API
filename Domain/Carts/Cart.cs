using Domain.Carts.ValueObjects;
using Domain.Customers;
using Domain.Kernal.Models;

namespace Domain.Carts;

public class Cart : AggregateRoot<Guid>
{
    private readonly HashSet<CartItem> _cartItems = new();

    public Guid CustomerId { get; set; }

    public IReadOnlyList<CartItem> CartItems => _cartItems.ToList();

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
