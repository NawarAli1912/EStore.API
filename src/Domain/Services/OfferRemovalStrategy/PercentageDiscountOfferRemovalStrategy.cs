using Domain.Errors;
using Domain.Offers;
using Domain.Orders;
using SharedKernel.Primitives;

namespace Domain.Services.OfferRemovalStrategy;

internal sealed class PercentageDiscountOfferRemovalStrategy : OfferRemovalStrategy
{
    public PercentageDiscountOfferRemovalStrategy(Offer offer)
        : base(offer)
    {
    }

    public override Result<Updated> Handle(Order order, int quantity)
    {
        if (!order.RequestedOffers.Contains(_offer.Id))
        {
            return DomainError.Offers.NotPresentInOrder;
        }

        var parsedOffer = (PercentageDiscountOffer)_offer;
        var productId = parsedOffer.ProductId;

        var result = order.RemoveItems(productId, quantity, parsedOffer.Id);

        if (result.IsError)
        {
            return result.Errors;
        }

        order.RemoveRequestedOffer(_offer.Id);

        return Result.Updated;
    }
}
