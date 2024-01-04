using SharedKernel.Primitives;

namespace Domain.Offers.Events;
public sealed record OfferExpiredDomainEvent(Offer ExpiredOffer) :
    IDomainEvent;
