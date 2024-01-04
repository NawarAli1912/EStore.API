using Application.Common.ElasticSearch;
using Domain.ModelsSnapshots;
using Domain.Products.Events;
using MediatR;
using Nest;
using Serilog;

namespace Application.Products.EventsHandler;

public sealed class ProductCreatedDomainEventHandler(IElasticClient elasticClient)
    : INotificationHandler<ProductCreatedDomainEvent>
{
    private readonly IElasticClient _elasticClient = elasticClient;

    public async Task Handle(ProductCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var productSnapshot = ProductSnapshot.Snapshot(notification.Product);

        try
        {
            await _elasticClient.IndexAsync(productSnapshot,
                i => i.Index(ElasticSearchSettings.DefaultIndex), cancellationToken);
        }
        catch (Exception ex)
        {
            Log.Error($"{nameof(ProductCreatedDomainEventHandler)} failed with error {ex.Message}.");
        }
    }
}

