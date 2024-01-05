using Domain.Offers;
using Domain.Offers.Enums;

namespace Domain.Services.OffersPricingStartegy;
internal static class OfferPricingStrategyFactory
{
    public static IOfferPricingStrategy GetStrategy(Offer offer)
    {
        return offer.Type switch
        {
            OfferType.BundleDiscountOffer => new BundleDiscountOfferPricingStrategy(),
            OfferType.PercentageDiscountOffer => new PercentageDiscountOfferPricingStrategy(),
            _ => throw new InvalidOperationException("Unsupported offer type"),
        };
    }
}
