using Application.Common.ElasticSearch;
using Domain.DomainEvents;
using Domain.ModelsSnapshots;
using MediatR;
using Microsoft.Extensions.Logging;
using Nest;

namespace Application.Common.Events;
public sealed class ProductUpdatedDomainEventHandler(
            IElasticClient elasticClient,
            ILogger<ProductUpdatedDomainEventHandler> logger)
    : INotificationHandler<ProductUpdatedDomainEvent>
{
    private readonly IElasticClient _elasticClient = elasticClient;
    private readonly ILogger<ProductUpdatedDomainEventHandler> _logger = logger;
    public async Task Handle(ProductUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var result = await _elasticClient.UpdateAsync<ProductSnapshot>(
            notification.Product.Id,
            u => u.Index(ElasticSearchSettings.DefaultIndex)
            .Doc(notification.Product)
            .Refresh(Elasticsearch.Net.Refresh.True));

        if (!result.IsValid)
        {
            _logger.LogError("Error updating the product.");
        }
    }
}
