using Domain.Carts.ValueObjects;
using Domain.Customers;

namespace Domain.Carts;

public class Cart
{
    private Cart(Guid id, Guid customerId)
    {
        Id = id;
        CustomerId = customerId;
    }

    private readonly HashSet<CartItem> _cartItems = new();

    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }

    public IReadOnlyList<CartItem> CartItems => _cartItems.ToList();

    public static Cart Create(Customer customer)
    {
        return new(Guid.NewGuid(), customer.Id);
    }

    private Cart()
    {
    }
}
