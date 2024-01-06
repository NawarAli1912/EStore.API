using Application.Common.Cache;
using Microsoft.Extensions.Caching.Memory;
using SharedKernel.Primitives;

namespace Infrastructure.Caching;
internal sealed class CacheService(IMemoryCache memoryCache)
    : ICacheService
{
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(5);

    private readonly IMemoryCache _memoryCache = memoryCache;

    public async Task CacheAsync<TResponse>(
        string key,
        TResponse response,
        TimeSpan? expiration)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? DefaultExpiration
        };

        await Task.Run(() => _memoryCache.Set(key, response, cacheEntryOptions));
    }

    public async Task<TResponse?> TryGetFromCacheAsync<TResponse>(string key)
    {
        var result = await Task.Run(() =>
        {
            if (_memoryCache.TryGetValue(key, out TResponse? cachedResponse))
            {
                return cachedResponse;
            }

            return default;
        });

        return result;
    }

    public async Task<T> GetOrCreateAsync<T>(
        Func<CancellationToken, Task<T>> factory,
        string key,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
        where T : IResult
    {
        T? result = await _memoryCache.GetOrCreateAsync(
            key,
            async entry =>
            {
                entry.SetAbsoluteExpiration(expiration ?? DefaultExpiration);
                var factoryValue = await factory(cancellationToken);
                if (factoryValue.IsError)
                {
                    throw new InvalidOperationException();
                }
                return factoryValue;
            });

        return result!;
    }
}

