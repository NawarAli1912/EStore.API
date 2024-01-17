using Application.Common;
using Application.Common.Cache;
using Domain.Products;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace Infrastructure.BackgroundJobs;

public sealed class CacheProductsJob : IJob
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICacheService _cacheService;

    public CacheProductsJob(ApplicationDbContext dbContext, ICacheService cacheService)
    {
        _dbContext = dbContext;
        _cacheService = cacheService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        List<Product> products = [];
        int skip = 0;
        int batchSize = 2000;
        while (true)
        {
            var batch = await _dbContext.Products
                .OrderBy(p => p.Id)
                .Skip(skip)
                .Take(batchSize)
                .ToListAsync();

            if (batch.Count == 0)
            {
                break;
            }

            foreach (var product in batch)
            {
                _cacheService.Set(
                    CacheKeys.ProductCacheKey(product.Id),
                    product,
                    TimeSpan.FromHours(12));
            }

            skip += batchSize;
        }
    }
}
