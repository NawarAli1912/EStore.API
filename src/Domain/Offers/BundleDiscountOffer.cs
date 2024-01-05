using Domain.Offers.Enums;

namespace Domain.Offers;

public sealed class BundleDiscountOffer : Offer
{
    private readonly List<Guid> _bundleProductsIds = [];

    public decimal Discount { get; private set; }

    public IReadOnlyList<Guid> BundleProductsIds => _bundleProductsIds;

    public static BundleDiscountOffer Create(
        string name,
        string description,
        List<Guid> bundleProductsIds,
        decimal percentage,
        DateOnly startDate,
        DateOnly endDate)
    {

        var offer = new BundleDiscountOffer
        {
            Name = name,
            Description = description,
            Discount = percentage,
            Type = OfferType.BundleDiscountOffer,
            StartsAt = startDate,
            EndsAt = endDate
        };

        foreach (var id in bundleProductsIds)
        {
            offer.AddProduct(id);
        }

        offer.UpdateStatus();

        return offer;
    }

    public void AddProduct(Guid productId)
    {
        _bundleProductsIds.Add(productId);
    }

    public override decimal CalculatePrice(Dictionary<Guid, decimal> products)
    {
        var discountFactor = 1.0M - Discount;
        var price = 0.0M;
        foreach (var id in _bundleProductsIds)
        {
            var productPrice = products[id];

            price += productPrice * discountFactor;
        }

        return price;
    }

    public override List<Guid> ListRelatedProductsIds()
    {
        return [.. _bundleProductsIds];
    }

    private BundleDiscountOffer() : base(Guid.NewGuid())
    {

    }
}
