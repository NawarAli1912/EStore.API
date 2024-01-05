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

    public Task<List<BundleDiscountOffer>?> ListBundleDiscountOffers()
    {
        string key = CacheKeys.BundleOffersCacheKey;

        return _memoryCache.GetOrCreateAsync(
            key,
            entry =>
                {
                    entry.SetAbsoluteExpiration(TimeSpan.FromHours(48));
                    return _context
                        .Offers.Where(o => o.Type == Domain.Offers.Enums.OfferType.BundleDiscountOffer)
                        .Cast<BundleDiscountOffer>()
                        .ToListAsync();
                }
            );
    }

    public Task<List<PercentageDiscountOffer>?> ListPercentageDiscountOffers()
    {
        string key = CacheKeys.PercentageOffersCacheKey;

        return _memoryCache.GetOrCreateAsync(
            key,
                entry =>
                {
                    entry.SetAbsoluteExpiration(TimeSpan.FromHours(48));
                    return _context
                        .Offers.Where(o => o.Type == Domain.Offers.Enums.OfferType.PercentageDiscountOffer)
                        .Cast<PercentageDiscountOffer>()
                        .ToListAsync();
                }
            );
    }
}
