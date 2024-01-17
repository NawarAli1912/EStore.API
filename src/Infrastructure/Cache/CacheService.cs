using Application.Common.Cache;
using Microsoft.Extensions.Caching.Memory;
using SharedKernel.Primitives;

namespace Infrastructure.Caching;
internal sealed class CacheService
    : ICacheService
{
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(5);

    private readonly IMemoryCache _memoryCache;

    public CacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public void Set<T>(
        string key,
        T item,
        TimeSpan? expiration)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            SlidingExpiration = expiration ?? DefaultExpiration
        };

        _memoryCache.Set(key, item, cacheEntryOptions);
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

    public TResponse? TryGet<TResponse>(string key)

    {
        if (_memoryCache.TryGetValue(key, out TResponse? cachedResponse))
        {
            return cachedResponse;
        }

        if (cachedResponse is null)
        {
            return default;
        }

        return cachedResponse;
    }

    public void Remove(string key)
    {
        _memoryCache.Remove(key);
    }
}

