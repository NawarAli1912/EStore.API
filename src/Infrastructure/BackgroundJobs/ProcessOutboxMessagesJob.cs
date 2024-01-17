using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Quartz;
using Serilog;
using SharedKernel.Primitives;

namespace Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
public sealed class ProcessOutboxMessagesJob : IJob
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPublisher _publisher;

    private const int MaxRetries = 3;

    public ProcessOutboxMessagesJob(ApplicationDbContext dbContext, IPublisher publisher)
    {
        _dbContext = dbContext;
        _publisher = publisher;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var serializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

        var messages = await _dbContext
                        .OutboxMessages
                        .Where(m => m.Done == false)
                        .OrderBy(m => m.Id)
                        .Take(20)
                        .ToListAsync(context.CancellationToken);

        foreach (var message in messages)
        {
            IDomainEvent? domainEvent = JsonConvert
                .DeserializeObject(message.Content, serializerSettings) as IDomainEvent;

            if (domainEvent is null)
            {
                continue;
            }

            try
            {
                await _publisher.Publish(domainEvent, context.CancellationToken);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to process domain event {message.Type} with exception {ex.Message}");

                message.RetryCount++;
                if (message.RetryCount == MaxRetries)
                {
                    message.Error = ex.Message.ToString();
                    message.Done = true;
                }
            }
            finally
            {
                message.ProcessedOnUtc = DateTime.UtcNow;
            }
        }

        await _dbContext.SaveChangesAsync();
    }
}
