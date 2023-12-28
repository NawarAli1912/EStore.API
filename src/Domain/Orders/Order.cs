using Domain.Customers;
using Domain.Orders.Entities;
using Domain.Orders.Enums;
using Domain.Orders.Errors;
using Domain.Orders.ValueObjects;
using Domain.Products;
using SharedKernel.Enums;
using SharedKernel.Primitives;

namespace Domain.Orders;

public sealed class Order : AggregateRoot, IAuditableEntity
{
    private readonly HashSet<LineItem> _lineItems = [];

    public Guid CustomerId { get; private set; }

    public OrderStatus Status { get; private set; }

    public ShippingInfo ShippingInfo { get; private set; }

    public decimal TotalPrice { get; private set; }

    public IReadOnlySet<LineItem> LineItems => _lineItems;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime ModifiedAtUtc { get; set; }

    public static Order Create(
        Customer customer,
        ShippingInfo shippingInfo)
    {

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            Status = OrderStatus.Pending,
            ShippingInfo = shippingInfo
        };

        return order;

    }

    public void AddItems(Product product, int quantity)
    {
        for (var i = 0; i < quantity; ++i)
        {
            var lineItem = LineItem
                .Create(
                Guid.NewGuid(),
                product.Id,
                Id,
                product.CustomerPrice);

            _lineItems.Add(lineItem);
        }

        TotalPrice += product.CustomerPrice * quantity;
    }

    public Result<Updated> RemoveItems(Product product, int Quantity)
    {
        if (_lineItems.Count(li => li.ProductId == product.Id) < Quantity)
        {
            return DomainError.LineItem.ExceedsAvailableQuantity(product.Id);
        }

        for (var i = 0; i < Quantity; ++i)
        {
            var lineItem = _lineItems
                .First(item => item.ProductId == product.Id);

            _lineItems.Remove(lineItem);

            TotalPrice -= lineItem.Price;
        }

        return Result.Updated;
    }

    public void UpdateShippingInfo(
        ShippingCompany? shippingCompany,
        string? shippingCompanyAddress,
        string? phoneNumber)
    {
        ShippingInfo.Update(
            shippingCompany,
            shippingCompanyAddress,
            phoneNumber);
    }

    public void Approve()
    {
        Status = OrderStatus.Approved;
    }

    public void Reject()
    {
        Status = OrderStatus.Rejected;
    }

    private Order() : base(Guid.NewGuid())
    {
    }
}
