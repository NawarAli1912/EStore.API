using Domain.Offers;
using Domain.Products;

namespace Domain.Services.OffersPricingStartegy;
internal sealed class PercentageDiscountOfferPricingStrategy : OfferProductsPricingStrategy
{
    public PercentageDiscountOfferPricingStrategy(
        Offer offer,
        IDictionary<Guid, Product> productDict) : base(offer, productDict)
    {
    }

    public override Dictionary<Guid, decimal> ComputeProductsPrices()
    {
        var parsedOffer = (PercentageDiscountOffer)_offer;
        var product = _productDict[parsedOffer.ProductId];

        var discountPrice =
            product.CustomerPrice * (1.0M - parsedOffer.Discount);

        return new()
        {
            { product.Id, discountPrice }
        };
    }
}
