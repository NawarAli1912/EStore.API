using Domain.Offers;
using Domain.Orders;
using Domain.Products;
using Domain.Services.OffersPricingStartegy;
using SharedKernel.Enums;
using SharedKernel.Primitives;

namespace Domain.Services.OfferAdditionStrategy;
internal sealed class BundleDiscountOfferAdditionStrategy : OfferAdditionStrategy
{
    public BundleDiscountOfferAdditionStrategy(
        Offer offer,
        Dictionary<Guid, Product> productDict) : base(offer, productDict)
    {
    }

    public override Result<Updated> Handle(Order order, int quantity)
    {
        List<Error> errors = [];

        var pricingStartegy = OfferPricingStrategyFactory.GetStrategy(_offer);
        var productToPrice = pricingStartegy.Handle(_offer, _productDict);

        foreach (var item in productToPrice)
        {
            var addItemResult = order.AddItems(
                item.Key,
                item.Value,
                quantity,
                ItemType.Offer,
                _offer.Id);

            if (addItemResult.IsError)
            {
                errors.AddRange(addItemResult.Errors);
            }
        }

        order.AddRequestedOffer(_offer.Id);

        return Result.Updated;
    }
}
