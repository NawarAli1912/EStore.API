using Domain.Offers.Enums;
using SharedKernel.Primitives;

namespace Domain.Offers;
public abstract class Offer : AggregateRoot<Guid>
{
    public string Name { get; protected set; }

    public string Description { get; protected set; }

    public OfferType Type { get; protected set; }

    protected Offer(Guid id) : base(id)
    {
    }
}
