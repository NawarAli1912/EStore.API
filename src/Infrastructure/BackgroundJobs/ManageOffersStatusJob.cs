using Domain.Offers;
using Domain.Offers.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
public sealed class ManageOffersStatusJob(ApplicationDbContext dbContext) : IJob
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task Execute(IJobExecutionContext context)
    {
        var query = _dbContext
            .Offers
            .Where(o => o.Status != OfferStatus.Expired);

        int batchSize = 100;
        int totalCount = await query.CountAsync();
        int batches = (int)Math.Ceiling((double)totalCount / batchSize);

        for (int i = 0; i < batches; ++i)
        {
            var offers = await query
                .Skip(i * batchSize)
                .Take(batchSize)
                .ToListAsync();

            AdjustStauts(offers);

            await _dbContext.SaveChangesAsync();
        }
    }

    private void AdjustStauts(List<Offer> offers)
    {
        foreach (var offer in offers)
        {
            offer.UpdateStatus();
        }
    }
}
