using Domain.Offers.Enums;
using Domain.Offers.Events;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Common.Events;
public sealed class OfferCreatedDomainEventHandler(IMemoryCache memoryCache)
        : INotificationHandler<OfferCreatedDominaEvent>
{
    private readonly IMemoryCache _memoryCache = memoryCache;

    public async Task Handle(OfferCreatedDominaEvent notification,
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
