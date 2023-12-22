using Application.Common.Cache;
using Microsoft.Extensions.Caching.Memory;
using SharedKernel.Primitives;

namespace Infrastructure.Caching;
internal sealed class CacheService(IMemoryCache memoryCache)
    : ICacheService
{
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(5);

    private readonly IMemoryCache _memoryCache = memoryCache;

    public async Task<T> GetOrCreateAsync<T>(
        Func<CancellationToken, Task<T>> factory,
        string key,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        T? result = await _memoryCache.GetOrCreateAsync(
            key,
            entry =>
            {
                entry.SetAbsoluteExpiration(expiration ?? DefaultExpiration);
                return factory(cancellationToken);
            });

        return result!;
    }

    public async Task<T> CreateAsync<T>(
        Func<CancellationToken, Task<T>> factory,
        string key,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        T? result = await _memoryCache.GetOrCreateAsync(
            key,
            entry =>
            {
                entry.SetAbsoluteExpiration(expiration ?? DefaultExpiration);
                return factory(cancellationToken);
            });

        return result!;
    }

    // Method for getting the cached item if it exists
    public async Task<T?> GetAsync<T>(string key)
        where T : class
    {
        var result = await Task.Run(() =>
        {
            var value = _memoryCache.Get<T>(key);

            return value;
        });

        return result is Result<T> ret && ret.Errors.Count > 0 ? null : result;
    }
}

