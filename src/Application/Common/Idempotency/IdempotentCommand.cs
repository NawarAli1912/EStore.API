using SharedKernel.Messaging;

namespace Application.Common.Idempotency;

public abstract record IdempotentCommand<T>(Guid RequestId) : IRequest<T>;
