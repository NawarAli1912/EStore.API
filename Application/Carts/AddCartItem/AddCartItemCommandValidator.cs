using FluentValidation;

namespace Application.Carts.AddCartItem;
public sealed class AddCartItemCommandValidator : AbstractValidator<AddCartItemCommand>
{
    public AddCartItemCommandValidator()
    {
        RuleFor(command => command.Quantity)
            .NotNull()
            .GreaterThan(0);
    }
}
