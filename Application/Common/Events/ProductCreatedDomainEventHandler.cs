using Application.Common.ElasticSearch;
using Domain.DomainEvents;
using MediatR;
using Nest;
using Serilog;

namespace Application.Common.Events;

public sealed class ProductCreatedDomainEventHandler(IElasticClient elasticClient)
    : INotificationHandler<ProductCreatedDomainEvent>
{
    private readonly IElasticClient _elasticClient = elasticClient;

    public async Task Handle(ProductCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var productSnapshot = notification.Product;

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
