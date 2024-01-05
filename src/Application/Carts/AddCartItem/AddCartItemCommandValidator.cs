using FluentValidation;

namespace Application.Carts.AddCartItem;
public sealed class AddCartItemCommandValidator : AbstractValidator<AddCartItemCommand>
{
    public AddCartItemCommandValidator()
    {
        RuleFor(command => command.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required.");

        RuleFor(command => command.Quantity)
            .NotNull()
            .GreaterThan(0);

        RuleFor(command => command)
           .Must(HaveEitherProductIdOrOfferIdButNotBoth)
           .WithMessage("Either ProductId or OfferId must be specified, but not both.");
    }

    private bool HaveEitherProductIdOrOfferIdButNotBoth(AddCartItemCommand command)
    {
        if (command.OfferId.HasValue && command.ProductId.HasValue ||
            command.ProductId is null && command.OfferId is null)
        {
            return false;
        }

        return true;
    }
}
