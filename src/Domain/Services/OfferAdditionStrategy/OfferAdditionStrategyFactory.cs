using Domain.Offers;
using Domain.Offers.Enums;
using Domain.Products;

namespace Domain.Services.OfferAdditionStrategy;
public static class OfferAdditionStrategyFactory
{
    public static OfferAdditionStrategy GetStrategy(Offer offer, Dictionary<Guid, Product> productDict)
    {
        return offer.Type switch
        {
            OfferType.BundleDiscountOffer => new BundleDiscountOfferAdditionStrategy(offer, productDict),
            OfferType.PercentageDiscountOffer => new PercentageDiscountOfferAdditionStrategy(offer, productDict),
            _ => throw new InvalidOperationException("Unsupported offer type"),
        };
    }
}
