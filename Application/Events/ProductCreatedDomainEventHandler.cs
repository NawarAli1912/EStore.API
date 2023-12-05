using Application.Common.ModelsSnapshots;
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
        var product = ProductSnapshot.Snapshot(notification.Product);

        var result = await _elasticClient.IndexAsync(product, i => i.Index("product"), cancellationToken);

        if (!result.IsValid)
        {
            _logger.LogError("Error indexing the product");
        }
    }
}
