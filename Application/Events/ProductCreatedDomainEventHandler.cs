using Application.ElasticSearch;
using Domain.DomainEvents;
using MediatR;
using Microsoft.Extensions.Logging;
using Nest;

namespace Application.Events;

public sealed class ProductCreatedDomainEventHandler(IElasticClient elasticClient, ILogger<ProductCreatedDomainEventHandler> logger)
    : INotificationHandler<ProductCreatedDomainEvent>
{
    private readonly IElasticClient _elasticClient = elasticClient;
    private readonly ILogger<ProductCreatedDomainEventHandler> _logger = logger;

    public async Task Handle(ProductCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var productSnapshot = notification.Product;

        var result = await _elasticClient.IndexAsync(productSnapshot,
            i => i.Index(ElasticSearchSettings.DefaultIndex), cancellationToken);

        if (!result.IsValid)
        {
            _logger.LogError("Error indexing the product");
        }
    }
}
