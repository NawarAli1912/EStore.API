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

    TResponse? TryGet<TResponse>(string key);


    void Set<T>(
        string key,
        T item,
        TimeSpan? expiration = null);

    void Remove(string key);
}
