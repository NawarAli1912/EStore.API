namespace Application.Common.Cache;

public interface ICacheService
{
    Task<T> CreateAsync<T>(Func<CancellationToken, Task<T>> factory, string key, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

    Task<T?> GetAsync<T>(string key)
        where T : class;

    Task<T> GetOrCreateAsync<T>(
        Func<CancellationToken, Task<T>> factory,
        string key,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default);
}
