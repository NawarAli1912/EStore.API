using Domain.Offers;
using Domain.Products;

namespace Domain.Services.OffersPricingStartegy;
internal sealed class BundleDiscountOfferPricingStrategy : IOfferPricingStrategy
{
    public Dictionary<Guid, decimal> Handle(Offer offer, IDictionary<Guid, Product> productDict)
    {
        Dictionary<Guid, decimal> result = [];

        var parsedOffer = (BundleDiscountOffer)offer;

        foreach (var productId in parsedOffer.BundleProductsIds)
        {
            var product = productDict[productId];
            var discountPrice = product.CustomerPrice * (1.0M - parsedOffer.Discount);

            result.Add(product.Id, discountPrice);
        }

        return result;
    }
}
