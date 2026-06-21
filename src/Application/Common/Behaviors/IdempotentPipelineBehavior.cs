using Application.Common.Idempotency;
using SharedKernel.Messaging;
using SharedKernel.Primitives;

namespace Application.Common.Behaviors;
public sealed class IdempotentPipelineBehavior<TRequest, TResponse>(IIdempotencyService idempotencyService)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IdempotentCommand<TResponse>
    where TResponse : IResult
{
    private readonly IIdempotencyService _idempotencyService = idempotencyService;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        List<Error> errors = [];

        if (await _idempotencyService.RequestExists(request.RequestId))
        {
            errors.Add(Error.Validation("Idempotency.AlreadyExists", "Idempotency key already exists."));
        }

        if (errors.Count > 0)
        {
            return (dynamic)errors;
        }

        await _idempotencyService.CreateRequest(request.RequestId, typeof(TRequest).Name);

        return await next();
    }
}
