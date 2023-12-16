using SharedKernel.Models;

namespace Domain.Orders.Entities;

public sealed class LineItem : Entity<Guid>
{
    private LineItem(
        Guid id,
        Guid productId,
        Guid orderId,
        decimal price) : base(id)
    {
        ProductId = productId;
        OrderId = orderId;
        Price = price;
    }

    public Guid ProductId { get; private set; }

    public Guid OrderId { get; private set; }

    public decimal Price { get; private set; } = default!;

    internal static LineItem Create(
        Guid id,
        Guid productId,
        Guid orderId,
        decimal price)
    {
        return new LineItem(id, productId, orderId, price);
    }

    private LineItem() : base(Guid.NewGuid())
    {
    }
}
