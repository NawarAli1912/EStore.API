using Domain.Customers;
using Domain.Orders.Entities;
using Domain.Orders.Enums;
using Domain.Orders.Errors;
using Domain.Products;
using SharedKernel;
using SharedKernel.Enums;
using SharedKernel.Models;

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

    public static Order Create(
        Customer customer,
        ShippingInfo shippingInfo)
    {

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow,
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

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Order() : base(Guid.NewGuid())
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }
}
