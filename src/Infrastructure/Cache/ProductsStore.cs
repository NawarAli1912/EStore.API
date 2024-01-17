using Application.Common.Cache;
using Domain.Products;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace Infrastructure.Cache;
public sealed class ProductsStore : IProductsStore
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ICacheService _cacheService;

    public ProductsStore(ApplicationDbContext dbContext, ICacheService cacheService)
    {
        _dbContext = dbContext;
        _cacheService = cacheService;
    }

    public async Task<List<Product>> GetByIds(ISet<Guid> requestedIds, CancellationToken cancellationToken)
    {
        var productsBag = new ConcurrentBag<Product>();
        var foundInCacheIds = new ConcurrentBag<Guid>();

        Parallel.ForEach(requestedIds, (id) =>
        {
            var cacheKey = $"product-{id}";
            var product = _cacheService.TryGet<Product>(cacheKey); // Assuming synchronous cache access

            if (product is null)
            {
                return;
            }

            productsBag.Add(product);
            foundInCacheIds.Add(id);

        });

        var remainingIds = requestedIds
                .Except(foundInCacheIds)
                .ToHashSet();

        if (remainingIds.Count == 0)
        {
            return [.. productsBag];
        }

        var missingProducts = await _dbContext.Products
                .Include(p => p.Categories)
                .Where(p => remainingIds.Contains(p.Id))
                .ToListAsync(cancellationToken);

        var result = productsBag.ToList();
        result.AddRange(missingProducts);

        return result;
    }

}
