using Domain.Offers;
using Domain.Products;

namespace Domain.Services.OffersPricingStartegy;
public interface IOfferPricingStrategy
{
    Dictionary<Guid, decimal> Handle(
        Offer offer,
        IDictionary<Guid, Product> productDict);
}
