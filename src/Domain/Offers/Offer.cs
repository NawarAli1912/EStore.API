using Domain.Offers.Enums;
using Domain.Offers.Events;
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

    public void UpdateStatus()
    {
        DateOnly currentDate = DateOnly.FromDateTime(DateTime.UtcNow);

        if (currentDate >= StartsAt && currentDate <= EndsAt)
        {
            Status = OfferStatus.Published;
        }
        else if (currentDate > EndsAt)
        {
            Status = OfferStatus.Expired;
        }
        else
        {
            Status = OfferStatus.Draft;
        }

        if (Status == OfferStatus.Expired)
        {
            RaiseDomainEvent(new OfferExpiredDomainEvent(this));
        }
    }

    public abstract decimal CalculatePrice(Dictionary<Guid, decimal> products);
}
