using Application.Common;
using Domain.Offers.Events;
using SharedKernel.Messaging;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Offers.EventsHandlers;
public sealed class OfferCreatedDomainEventHandler(IMemoryCache memoryCache)
        : INotificationHandler<OfferCreatedDomainEvent>
{
    private readonly IMemoryCache _memoryCache = memoryCache;

    public Task Handle(OfferCreatedDomainEvent notification,
        CancellationToken cancellationToken)
    {
        _memoryCache.Remove(CacheKeys.OffersCacheKey);
        return Task.CompletedTask;
    }
}
