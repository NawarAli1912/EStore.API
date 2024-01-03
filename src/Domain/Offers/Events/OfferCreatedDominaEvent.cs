using SharedKernel.Primitives;

namespace Domain.Offers.Events;
public sealed record OfferCreatedDominaEvent(Offer Offer)
    : IDomainEvent;
