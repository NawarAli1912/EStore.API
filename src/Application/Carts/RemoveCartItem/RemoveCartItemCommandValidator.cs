using FluentValidation;

namespace Application.Carts.RemoveCartItem;

public sealed class RemoveCartItemCommandValidator : AbstractValidator<RemoveCartItemCommand>
{
    public RemoveCartItemCommandValidator()
    {
        RuleFor(command => command.Quantity)
            .NotNull()
            .GreaterThan(0);
    }
}