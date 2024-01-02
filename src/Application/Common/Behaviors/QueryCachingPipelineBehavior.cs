using Application.Common.Cache;
using MediatR;
using SharedKernel.Primitives;

namespace Application.Common.Behaviors;
public sealed class QueryCachingPipelineBehavior<TRequest, TResponse>
    (ICacheService cachedService)
        : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICachedQuery
    where TResponse : IResult
{
    private readonly ICacheService _cachedService = cachedService;

    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        return await _cachedService.GetOrCreateAsync(
            _ => next(),
            request.Key,
            request.Expiration,
            cancellationToken);
    }
}
