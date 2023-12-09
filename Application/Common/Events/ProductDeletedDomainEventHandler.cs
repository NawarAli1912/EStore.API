using Domain.DomainEvents;
using Domain.ModelsSnapshots;
using MediatR;
using Microsoft.Extensions.Logging;
using Nest;
using Serilog;

namespace Application.Common.Events;

public class ProductDeletedDomainEventHandler(IElasticClient elasticClient, ILogger<ProductDeletedDomainEventHandler> logger)
    : INotificationHandler<ProductDeletedDomainEvent>
{
    private readonly IElasticClient _elasticClient = elasticClient;
    private readonly ILogger<ProductDeletedDomainEventHandler> _logger = logger;

    public async Task Handle(ProductDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            await _elasticClient.DeleteAsync<ProductSnapshot>
            (notification.Id,
            ct: cancellationToken);
        }
        catch (Exception ex)
        {
            Log.Error($"{nameof(ProductDeletedDomainEventHandler)} failed with error {ex.Message}.");
        }
    }
}
