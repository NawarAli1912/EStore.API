using Domain.Offers;
using Domain.Orders;
using Domain.Products;
using SharedKernel.Primitives;

namespace Domain.Services.OfferAdditionStrategy;

public abstract class OfferAdditionStrategy
{
    protected readonly IDictionary<Guid, Product> _productDict;

    protected readonly Offer _offer;

    protected OfferAdditionStrategy(Offer offer, Dictionary<Guid, Product> productDict)
    {
        _productDict = productDict ?? throw new ArgumentNullException(nameof(productDict));
        _offer = offer;
    }

    public abstract Result<Updated> Handle(Order order, int quantity);

}
