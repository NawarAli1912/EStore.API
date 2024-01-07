using Domain.Offers;
using Domain.Orders;
using SharedKernel.Primitives;

namespace Domain.Services.OfferRemovalStrategy;
public abstract class OfferRemovalStrategy
{
    protected readonly Offer _offer;

    protected OfferRemovalStrategy(Offer offer)
    {
        _offer = offer;
    }

    public abstract Result<Updated> Handle(Order order, int quantity);


}
