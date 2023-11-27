using Domain.Kernal.ValueObjects;

namespace Domain.Orders.Entities;

public class LineItem
{
    private LineItem(Guid id, Guid productId, Guid orderId, Money price)
    {
        Id = id;
        ProductId = productId;
        OrderId = orderId;
        Price = price;
    }

    public Guid Id { get; private set; }

    public Guid ProductId { get; private set; }

    public Guid OrderId { get; private set; }

    public Money Price { get; private set; } = default!;

    public static LineItem Create(Guid id, Guid productId, Guid orderId, Money price)
    {
        return new LineItem(id, productId, orderId, price);
    }

    private LineItem()
    {
    }
}
