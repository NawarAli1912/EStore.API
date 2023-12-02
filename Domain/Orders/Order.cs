using Domain.Customers;
using Domain.Kernal.Models;
using Domain.Orders.Entities;
using Domain.Orders.Enums;
using Domain.Products;

namespace Domain.Orders;

public sealed class Order : AggregateRoot<Guid>
{
    private readonly HashSet<LineItem> _lineItems = [];

    public Guid CustomerId { get; private set; }

    public OrderStatus Status { get; private set; }

    public IReadOnlySet<LineItem> LineItems => _lineItems.ToHashSet();

    public static Order Create(Customer customer)
    {
        return new Order
        {
            CustomerId = customer.Id
        };
    }

    public void Add(Product product)
    {
        var lineItem = LineItem.Create(Guid.NewGuid(), product.Id, Id, product.CustomerPrice);

        _lineItems.Add(lineItem);
    }

    private Order() : base(Guid.NewGuid())
    {
    }
}
