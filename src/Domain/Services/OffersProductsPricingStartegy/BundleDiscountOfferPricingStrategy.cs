using Domain.Offers;
using Domain.Products;

namespace Domain.Services.OffersPricingStartegy;
internal sealed class BundleDiscountOfferPricingStrategy : OfferProductsPricingStrategy
{
    public BundleDiscountOfferPricingStrategy(
        Offer offer,
        IDictionary<Guid, Product> productDict) : base(offer, productDict)
    {
    }

    public override Dictionary<Guid, decimal> ComputeProductsPrices()
    {
        Dictionary<Guid, decimal> result = [];

        var parsedOffer = (BundleDiscountOffer)_offer;

        foreach (var productId in parsedOffer.BundleProductsIds)
        {
            var product = _productDict[productId];
            var discountPrice = product.CustomerPrice * (1.0M - parsedOffer.Discount);

            result.Add(product.Id, discountPrice);
        }

        return result;
    }
}
