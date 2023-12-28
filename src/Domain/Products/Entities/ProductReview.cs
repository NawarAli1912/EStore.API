using SharedKernel.Primitives;

namespace Domain.Products.Entities;
public sealed class ProductReview : Entity
{
    public Guid ProductId { get; private set; }

    public Guid CustomerId { get; private set; }

    public string Comment { get; private set; }

    internal static ProductReview Create(Guid id)
    {
        return new ProductReview
        {
            Id = id
        };
    }

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

    internal void UpdateComment(string comment)
    {
        Comment = comment;
    }

    private ProductReview(
        Guid id,
        Guid productId,
        Guid customerId,
        string comment) : base(id)
    {

    }


    private ProductReview() : base(Guid.NewGuid())
    {

    }
}
