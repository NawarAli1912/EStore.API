using Domain.Offers;
using Domain.Offers.Enums;
using Domain.Products;

namespace Domain.Services.OffersPricingStartegy;
public static class OfferProductsPricingStrategyFactory
{
    public static OfferProductsPricingStrategy GetStrategy(Offer offer, IDictionary<Guid, Product> productDict)
    {
        return offer.Type switch
        {
            OfferType.BundleDiscountOffer => new BundleDiscountOfferPricingStrategy(offer, productDict),
            OfferType.PercentageDiscountOffer => new PercentageDiscountOfferPricingStrategy(offer, productDict),
            _ => throw new InvalidOperationException("Unsupported offer type"),
        };
    }
}
