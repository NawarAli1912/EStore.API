using Application.Common.ElasticSearch;
using Domain.ModelsSnapshots;
using Domain.Products.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using Nest;
using Serilog;

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
        try
        {
            await _elasticClient.UpdateAsync<ProductSnapshot>(notification.Product.Id, u => u.Index(ElasticSearchSettings.DefaultIndex)
                .Doc(notification.Product)
                .Refresh(Elasticsearch.Net.Refresh.True), cancellationToken);

        }
        catch (Exception ex)
        {
            Log.Error($"{nameof(ProductUpdatedDomainEventHandler)} failed with error {ex.Message}.");
        }
    }
}
