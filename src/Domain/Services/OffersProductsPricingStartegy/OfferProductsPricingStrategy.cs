using Domain.Offers;
using Domain.Products;

namespace Domain.Services.OffersPricingStartegy;
public abstract class OfferProductsPricingStrategy
{
    protected readonly Offer _offer;

    protected readonly IDictionary<Guid, Product> _productDict;

    protected OfferProductsPricingStrategy(Offer offer, IDictionary<Guid, Product> productDict)
    {
        _offer = offer;
        _productDict = productDict ??
            throw new ArgumentNullException(nameof(productDict)); ;
    }

    public abstract Dictionary<Guid, decimal> ComputeProductsPrices();
}
