using Application.Common;
using Application.Common.Repository;
using Domain.Offers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Persistence.Repository;
public sealed class OffersRepository(ApplicationDbContext context, IMemoryCache memoryCache)
    : IOffersRepository
{
    private readonly ApplicationDbContext _context = context;
    private readonly IMemoryCache _memoryCache = memoryCache;



    public Task<List<Offer>?> List()
    {
        string key = CacheKeys.OffersCacheKey;

        return _memoryCache.GetOrCreateAsync(
            key,
            entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromHours(48));
                return _context
                        .Offers
                        .ToListAsync();
            });
    }
}
