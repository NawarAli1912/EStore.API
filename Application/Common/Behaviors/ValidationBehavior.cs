using Domain.Kernal;
using FluentValidation;
using MediatR;

namespace Application.Common.Behaviors;

public class ValidationBehavior<T, R> : IPipelineBehavior<T, R>
    where T : IRequest<R>
    where R : IResult
{
    private readonly IValidator<T>? _validator;

    public ValidationBehavior(IValidator<T>? validator = null)
    {
        _validator = validator;
    }

    public async Task<R> Handle(
        T request,
        RequestHandlerDelegate<R> next,
        CancellationToken cancellationToken)
    {
        if (_validator is null)
        {
            return await next();
        }

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (validationResult.IsValid)
        {
            return await next();

        }

        var errors = validationResult.Errors.
            ConvertAll(e => Error.Validation(e.ErrorCode, e.ErrorMessage));

        // Totally safe to use dynamic because of the generic constraints
        // the result can always be mapped to <Type> Error </Type>
        return (dynamic)errors;
    }
}
