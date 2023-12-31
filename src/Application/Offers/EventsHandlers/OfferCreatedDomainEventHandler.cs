﻿using Application.Common;
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
            _memoryCache.Remove(CacheKeys.OffersCacheKey);
        }, cancellationToken);
    }
}
