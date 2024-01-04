using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using SharedKernel.Primitives;

namespace Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
public sealed class ProcessOutboxMessagesJob(
    ApplicationDbContext dbContext,
    IPublisher publisher,
    ILogger<ProcessOutboxMessagesJob> logger) : IJob
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly IPublisher _publisher = publisher;
    private readonly ILogger<ProcessOutboxMessagesJob> _logger = logger;

    private const int MaxRetries = 3;

    public async Task Execute(IJobExecutionContext context)
    {
        var messages = await _dbContext
                        .OutboxMessages
                        .Where(m => m.ProcessedOnUtc == null)
                        .OrderBy(m => m.Id)
                        .Take(20)
                        .ToListAsync(context.CancellationToken);

        foreach (var message in messages)
        {
            IDomainEvent? domainEvent = JsonConvert
                .DeserializeObject(message.Content, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                }) as IDomainEvent;

            if (domainEvent is null)
            {
                continue;
            }

            try
            {
                if (message.RetryCount > MaxRetries)
                {
                    continue;
                }

                await _publisher.Publish(domainEvent, context.CancellationToken);
                message.ProcessedOnUtc = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to process domain event {message.Type}");
                message.RetryCount++;
                if (message.RetryCount == MaxRetries)
                {
                    message.Error = ex.Message.ToString();
                }
            }
        }

        await _dbContext.SaveChangesAsync();
    }
}
