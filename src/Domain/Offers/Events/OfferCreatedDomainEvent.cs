using SharedKernel.Primitives;

namespace Domain.Offers.Events;
public sealed record OfferCreatedDomainEvent(Offer Offer)
    : IDomainEvent;
