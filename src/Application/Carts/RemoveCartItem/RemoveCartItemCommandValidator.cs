using FluentValidation;

namespace Application.Carts.RemoveCartItem;

public sealed class RemoveCartItemCommandValidator : AbstractValidator<RemoveCartItemCommand>
{
    public RemoveCartItemCommandValidator()
    {
        RuleFor(command => command.CustomerId)
           .NotEmpty()
           .WithMessage("Customer ID is required.");

        RuleFor(command => command.Quantity)
            .NotNull()
            .GreaterThan(0);
    }
}