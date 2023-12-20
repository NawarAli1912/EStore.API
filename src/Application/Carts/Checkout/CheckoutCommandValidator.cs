using FluentValidation;

namespace Application.Carts.Checkout;

public sealed class CheckoutCommandValidator
    : AbstractValidator<CheckoutCommand>
{
    public CheckoutCommandValidator()
    {
        RuleFor(command => command.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required.");

        RuleFor(command => command.ShippingCompany)
            .NotNull()
            .WithMessage("Shipping company is required.");

        RuleFor(command => command.ShippingCompanyLocation)
                .NotEmpty()
                .WithMessage("Shipping company location is required.")
                .MaximumLength(255)
                .WithMessage("Shipping company location should not exceed 255 characters.");

        RuleFor(command => command.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+9639[5-9]\d{7}$")
            .WithMessage("Invalid phone number.");
    }
}
