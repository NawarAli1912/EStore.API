using Domain.DomainEvents;
using Domain.ModelsSnapshots;
using MediatR;
using Microsoft.Extensions.Logging;
using Nest;

namespace Application.Common.Events;

public class ProductDeletedDomainEventHandler(IElasticClient elasticClient, ILogger<ProductDeletedDomainEventHandler> logger)
    : INotificationHandler<ProductDeletedDomainEvent>
{
    private readonly IElasticClient _elasticClient = elasticClient;
    private readonly ILogger<ProductDeletedDomainEventHandler> _logger = logger;

    public async Task Handle(ProductDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        var result = await _elasticClient.DeleteAsync<ProductSnapshot>
            (notification.Id,
            ct: cancellationToken);

        if (!result.IsValid)
        {
            _logger.LogError("Error deleting the product.");
        }
    }
}
