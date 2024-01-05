using Domain.Offers;
using Domain.Products;

namespace Domain.Services.OffersPricingStartegy;
internal interface IOfferPricingStrategy
{
    Dictionary<Guid, decimal> Handle(
        Offer offer,
        Dictionary<Guid, Product> productDict);
}
