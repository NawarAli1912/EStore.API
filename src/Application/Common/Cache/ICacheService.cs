using SharedKernel.Primitives;

namespace Application.Common.Cache;

public interface ICacheService
{
    Task<T> GetOrCreateAsync<T>(
        Func<CancellationToken, Task<T>> factory,
        string key,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
        where T : IResult;

    Task<TResponse?> TryGetFromCacheAsync<TResponse>(string key);


    Task CacheAsync<TResponse>(
        string key,
        TResponse response,
        TimeSpan? expiration = null);
}
