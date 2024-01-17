using Application.Common;
using Application.Common.Cache;
using Application.Common.ElasticSearch;
using Domain.ModelsSnapshots;
using Domain.Products.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using Nest;

namespace Application.Products.EventsHandler;
public sealed class ProductUpdatedDomainEventHandler(
            IElasticClient elasticClient,
            ILogger<ProductUpdatedDomainEventHandler> logger,
            ICacheService cacheService)
    : INotificationHandler<ProductUpdatedDomainEvent>
{
    private readonly IElasticClient _elasticClient = elasticClient;
    private readonly ICacheService _cacheService = cacheService;
    private readonly ILogger<ProductUpdatedDomainEventHandler> _logger = logger;

    public async Task Handle(ProductUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var productSnapshot = ProductSnapshot.Snapshot(notification.Product);

        _cacheService.Set(
            CacheKeys.ProductCacheKey(notification.Product.Id),
            notification.Product,
            TimeSpan.FromHours(2));

        await _elasticClient.UpdateAsync<ProductSnapshot>(notification.Product.Id, u => u.Index(ElasticSearchSettings.DefaultIndex)
            .Doc(productSnapshot)
            .Refresh(Elasticsearch.Net.Refresh.True), cancellationToken);
    }
}
