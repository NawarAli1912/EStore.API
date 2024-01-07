using Domain.Errors;
using SharedKernel.Enums;
using SharedKernel.Primitives;

namespace Domain.Orders.Entities;

public sealed class LineItem : Entity
{
    public Guid ProductId { get; private set; }

    public Guid OrderId { get; private set; }

    public decimal Price { get; private set; } = default!;

    public ItemType Type { get; private set; } = ItemType.Product;

    public Guid? RelatedOfferId { get; private set; }

    internal static Result<LineItem> Create(
        Guid productId,
        Guid orderId,
        decimal price,
        ItemType type = ItemType.Product,
        Guid? relatedOfferId = default)
    {
        if (type == ItemType.Product &&
            relatedOfferId.HasValue)
        {
            return DomainError.LineItem.InvalidCreationData;
        }

        return new LineItem
        {
            ProductId = productId,
            OrderId = orderId,
            Price = price,
            Type = type,
            RelatedOfferId = relatedOfferId
        };
    }

    private LineItem() : base(Guid.NewGuid())
    {
    }
}
