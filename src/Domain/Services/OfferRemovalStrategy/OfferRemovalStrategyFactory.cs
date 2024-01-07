using Domain.Offers;
using Domain.Offers.Enums;

namespace Domain.Services.OfferRemovalStrategy;
public class OfferRemovalStrategyFactory
{
    public static OfferRemovalStrategy GetStrategy(Offer offer)
    {
        return offer.Type switch
        {
            OfferType.BundleDiscountOffer => new BundleDiscountOfferRemovalStrategy(offer),
            OfferType.PercentageDiscountOffer => new PercentageDiscountOfferRemovalStrategy(offer),
            _ => throw new InvalidOperationException("Unsupported offer type"),
        };
    }
}
