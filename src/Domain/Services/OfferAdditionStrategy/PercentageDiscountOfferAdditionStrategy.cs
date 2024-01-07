using Domain.Offers;
using Domain.Orders;
using Domain.Products;
using Domain.Services.OffersPricingStartegy;
using SharedKernel.Enums;
using SharedKernel.Primitives;

namespace Domain.Services.OfferAdditionStrategy;
internal sealed class PercentageDiscountOfferAdditionStrategy : OfferAdditionStrategy
{
    public PercentageDiscountOfferAdditionStrategy(
        Offer offer,
        Dictionary<Guid, Product> productDict) : base(offer, productDict)
    {
    }

    public override Result<Updated> Handle(Order order, int quantity)
    {
        List<Error> errors = [];

        var parsedOffer = (PercentageDiscountOffer)_offer;

        var pricingStartegy = OfferPricingStrategyFactory.GetStrategy(_offer);
        var productToPrice = pricingStartegy.Handle(_offer, _productDict);

        var productPrice = productToPrice[parsedOffer.ProductId];

        var addItemResult = order.AddItems(
            parsedOffer.ProductId,
            productPrice,
            quantity,
            ItemType.Offer,
            _offer.Id);

        if (addItemResult.IsError)
        {
            errors.AddRange(addItemResult.Errors);
        }

        order.AddRequestedOffer(_offer.Id);

        return Result.Updated;
    }
}
