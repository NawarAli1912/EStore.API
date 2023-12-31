using MediatR;

namespace Application.Common.Idempotency;

public abstract record IdempotentCommand<T>(Guid RequestId) : IRequest<T>;
