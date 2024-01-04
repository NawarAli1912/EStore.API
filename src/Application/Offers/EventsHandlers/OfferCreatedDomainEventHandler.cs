using Application.Common;
using Domain.Offers.Enums;
using Domain.Offers.Events;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Offers.EventsHandlers;
public sealed class OfferCreatedDomainEventHandler(IMemoryCache memoryCache)
        : INotificationHandler<OfferCreatedDomainEvent>
{
    private readonly IMemoryCache _memoryCache = memoryCache;

    public async Task Handle(OfferCreatedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        await Task.Run(() =>
        {
            if (notification.Offer.Type == OfferType.BundleDiscountOffer)
            {
                _memoryCache.Remove(CacheKeys.BundleOffersCacheKey);

            }
            if (notification.Offer.Type != OfferType.BundleDiscountOffer)
            {
                _memoryCache.Remove(CacheKeys.PercentageOffersCacheKey);
            }
        }, cancellationToken);
    }
}
