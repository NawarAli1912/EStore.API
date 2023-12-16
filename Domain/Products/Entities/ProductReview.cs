using SharedKernel.Models;

namespace Domain.Products.Entities;
public sealed class ProductReview : Entity<Guid>
{
    public Guid ProductId { get; private set; }

    public Guid CustomerId { get; private set; }

    public string Comment { get; private set; }


    public static ProductReview Create(
        Guid id,
        Guid productId,
        Guid customerId,
        string comment)
    {
        return new ProductReview(
            id,
            productId,
            customerId,
            comment);
    }

    private ProductReview(
        Guid id,
        Guid productId,
        Guid customerId,
        string comment)
        : base(id)
    {
        ProductId = productId;
        CustomerId = customerId;
        Comment = comment;
    }

    private ProductReview() : base(Guid.NewGuid())
    {

    }
}
