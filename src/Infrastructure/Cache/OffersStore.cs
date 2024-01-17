using Application.Common;
using Application.Common.Cache;
using Domain.Offers;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Cache;
public sealed class OffersStore
    : IOffersStore
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _memoryCache;

    public OffersStore(
        ApplicationDbContext context,
        IMemoryCache memoryCache)
    {
        _context = context;
        _memoryCache = memoryCache;
    }

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
