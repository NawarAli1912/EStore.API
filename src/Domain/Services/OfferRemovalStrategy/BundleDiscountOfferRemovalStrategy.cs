using Domain.Errors;
using Domain.Offers;
using Domain.Orders;
using SharedKernel.Primitives;

namespace Domain.Services.OfferRemovalStrategy;
internal sealed class BundleDiscountOfferRemovalStrategy : OfferRemovalStrategy
{
    public BundleDiscountOfferRemovalStrategy(Offer offer) : base(offer)
    {
    }

    public override Result<Updated> Handle(Order order, int quantity)
    {
        if (!order.RequestedOffers.Contains(_offer.Id))
        {
            return DomainError.Offers.NotPresentInOrder;
        }

        var parsedOffer = (BundleDiscountOffer)_offer;
        var productsIdsToRemove = parsedOffer.ListRelatedProductsIds();

        foreach (var productId in productsIdsToRemove)
        {
            var result = order.RemoveItems(productId, quantity, parsedOffer.Id);
            if (result.IsError)
            {
                return result.Errors;
            }
        }

        order.RemoveRequestedOffer(_offer.Id);

        return Result.Updated;
    }
}
