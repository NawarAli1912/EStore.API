using Application.Common;
using Application.Common.Cache;
using Application.Common.ElasticSearch;
using Domain.ModelsSnapshots;
using Domain.Products.Events;
using MediatR;
using Nest;

namespace Application.Products.EventsHandler;

public sealed class ProductCreatedDomainEventHandler(IElasticClient elasticClient, ICacheService cacheService)
    : INotificationHandler<ProductCreatedDomainEvent>
{
    private readonly IElasticClient _elasticClient = elasticClient;
    private readonly ICacheService _cacheService = cacheService;

    public async Task Handle(ProductCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var productSnapshot = ProductSnapshot.Snapshot(notification.Product);

        _cacheService.Set(
            CacheKeys.ProductCacheKey(notification.Product.Id),
            notification.Product,
            TimeSpan.FromHours(2));

        await _elasticClient.IndexAsync(productSnapshot,
            i => i.Index(ElasticSearchSettings.DefaultIndex), cancellationToken);

    }
}

