using Domain.Offers.Enums;
using SharedKernel.Primitives;

namespace Domain.Offers;
public abstract class Offer : AggregateRoot
{
    public string Name { get; protected set; } = string.Empty;

    public string Description { get; protected set; } = string.Empty;

    public OfferType Type { get; protected set; }

    public OfferStatus Status { get; protected set; }

    public DateOnly StartsAt { get; protected set; }

    public DateOnly EndsAt { get; protected set; }

    protected Offer(Guid id) : base(id)
    {
    }

    private Offer() : base(Guid.NewGuid())
    {
    }
}
