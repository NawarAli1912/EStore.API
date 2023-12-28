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

        var offer = new BundleDiscountOffer(Guid.NewGuid())
        {
            Name = name,
            Description = description,
            Discount = percentage,
            Type = OfferType.BundleDiscountOffer,
            StartsAt = startDate,
            EndsAt = endDate
        };

        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);

        offer.Status = OfferStatus.Draft;

        if (currentDate >= startDate && currentDate <= endDate)
        {
            offer.Status = OfferStatus.Published;
        }
        else if (currentDate > endDate)
        {
            offer.Status = OfferStatus.End;
        }

        foreach (var id in bundleProductsIds)
        {
            offer.AddProduct(id);
        }

        return offer;
    }

    public void AddProduct(Guid productId)
    {
        _bundleProductsIds.Add(productId);
    }

    public BundleDiscountOffer(Guid id) : base(id)
    {
    }
}
