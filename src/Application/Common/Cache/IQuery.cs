using MediatR;

namespace Application.Common.Cache;

public interface IQuery<TResponse> : IRequest<TResponse>
{
}

public interface ICachedQuery
{
    string Key { get; }

    TimeSpan? Expiration { get; }
}

public interface ICachedQuery<TResponse> : IQuery<TResponse>, ICachedQuery
{
}
