using Domain.Customers;
using Domain.Kernal;
using Domain.Kernal.Enums;
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

    public ShippingInfo ShippingInfo { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime ModifiedAt { get; private set; }

    public decimal TotalPrice { get; private set; }

    public IReadOnlySet<LineItem> LineItems => _lineItems;

    public static Result<Order> Create(
        Customer customer,
        ShippingCompany ShippingCompany,
        string ShippingComapnyLocation,
        string PhoneNumber)
    {

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };

        var shippingInfoResult = ShippingInfo
            .Create(order.Id, ShippingCompany, ShippingComapnyLocation, PhoneNumber);

        if (shippingInfoResult.IsError)
        {
            return shippingInfoResult.Errors;
        }

        order.ShippingInfo = shippingInfoResult.Value;

        return order;

    }

    public void AddItem(Product product)
    {
        var lineItem = LineItem
            .Create(
            Guid.NewGuid(),
            product.Id,
            Id,
            product.CustomerPrice);

        _lineItems.Add(lineItem);

        TotalPrice += product.CustomerPrice;
    }

    private Order() : base(Guid.NewGuid())
    {
    }
}
