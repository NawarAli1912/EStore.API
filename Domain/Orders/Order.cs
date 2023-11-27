using Domain.Customers;
using Domain.Orders.Entities;
using Domain.Products;

namespace Domain.Orders;

public class Order
{
    private Order(Guid id, Guid customerId)
    {
        Id = id;
        CustomerId = customerId;
        Status = OrderStatus.Pending;
    }

    private readonly HashSet<LineItem> _lineItems = new();

    public Guid Id { get; private set; }

    public Guid CustomerId { get; private set; }

    public OrderStatus Status { get; private set; }

    public IReadOnlyList<LineItem> LineItems => _lineItems.ToList();

    public static Order Create(Customer customer)
    {
        return new Order(Guid.NewGuid(), customer.Id);
    }

    public void Add(Product product)
    {
        var lineItem = LineItem.Create(Guid.NewGuid(), product.Id, Id, product.Price);

        _lineItems.Add(lineItem);
    }

    private Order()
    {
    }
}
