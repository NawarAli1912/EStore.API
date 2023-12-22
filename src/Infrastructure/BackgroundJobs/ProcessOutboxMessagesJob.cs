using Infrastructure.Persistence;
using Infrastructure.Persistence.Outbox;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using SharedKernel.Primitives;

namespace Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
public sealed class ProcessOutboxMessagesJob(
    ApplicationDbContext context,
    IPublisher publisher,
    ILogger<ProcessOutboxMessagesJob> logger) : IJob
{
    private readonly ApplicationDbContext _context = context;
    private readonly IPublisher _publisher = publisher;
    private readonly ILogger<ProcessOutboxMessagesJob> _logger = logger;

    public async Task Execute(IJobExecutionContext jobContext)
    {
        var messages = await _context
                        .Set<OutboxMessage>()
                        .Where(m => m.ProcessedOnUtc == null)
                        .Take(20)
                        .OrderBy(m => m.Id)
                        .ToListAsync(jobContext.CancellationToken);

        foreach (var message in messages)
        {
            var domainEvent = JsonConvert
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
                await _publisher.Publish(domainEvent, jobContext.CancellationToken);
                message.ProcessedOnUtc = DateTime.UtcNow;
            }
            catch (Exception)
            {
                _logger.LogError($"Failed to process domain event {message.Type}");
                /*message.RetryCount++;
                if (message.RetryCount >= MaxRetries)
                {
                    message.Error = ex.Message.ToString();
                }*/
            }
        }

        await _context.SaveChangesAsync();

    }
}
