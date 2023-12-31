using Application.Common.Idempotency;
using MediatR;
using SharedKernel.Primitives;

namespace Application.Common.Behaviors;
public sealed class IdempotentCommandsPipelineBehavior<TRequest, TResponse>(IIdemptencyService idemptoecyService)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IdempotentCommand<TResponse>
    where TResponse : IResult
{
    private readonly IIdemptencyService _idemptoecyService = idemptoecyService;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        List<Error> errors = [];

        if (await _idemptoecyService.RequestExists(request.RequestId))
        {
            errors.Add(Error.Validation("Idempotency.AlreadyExists", "Idempotency key already exists."));
        }

        if (errors.Count > 0)
        {
            return (dynamic)errors;
        }

        await _idemptoecyService.CreateRequest(request.RequestId, typeof(TRequest).Name);

        return await next();
    }
}
