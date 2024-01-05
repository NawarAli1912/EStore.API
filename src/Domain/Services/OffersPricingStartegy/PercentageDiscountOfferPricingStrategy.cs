using Domain.Offers;
using Domain.Products;

namespace Domain.Services.OffersPricingStartegy;
internal sealed class PercentageDiscountOfferPricingStrategy : IOfferPricingStrategy
{
    public Dictionary<Guid, decimal> Handle(
        Offer offer,
        Dictionary<Guid, Product> productDict)
    {
        var parsedOffer = (PercentageDiscountOffer)offer;
        var product = productDict[parsedOffer.ProductId];

        var discountPrice =
            product.CustomerPrice * (1.0M - parsedOffer.Discount);

        return new()
        {
            { product.Id, discountPrice }
        };
    }
}
