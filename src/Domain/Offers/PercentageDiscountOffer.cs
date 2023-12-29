using Domain.Offers.Enums;

namespace Domain.Offers;

public sealed class PercentageDiscountOffer : Offer
{
    public Guid ProductId { get; private set; }

    public decimal Discount { get; private set; }

    public static PercentageDiscountOffer Create(
        string name,
        string description,
        Guid productId,
        decimal percentage)
    {
        return new PercentageDiscountOffer
        {
            Name = name,
            Description = description,
            ProductId = productId,
            Discount = percentage,
            Type = OfferType.PercentageDiscountOffer
        };
    }

    private PercentageDiscountOffer() : base(Guid.NewGuid())
    {

    }
}
