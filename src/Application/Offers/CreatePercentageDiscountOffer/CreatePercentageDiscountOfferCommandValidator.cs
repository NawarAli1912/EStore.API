using FluentValidation;

namespace Application.Offers.CreatePercentageDiscountOffer;
internal class CreatePercentageDiscountOfferCommandValidator : AbstractValidator<CreatePercentageDiscountOfferCommand>
{
    public CreatePercentageDiscountOfferCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Length(2, 100).WithMessage("Name must be between 2 and 100 characters.");

        RuleFor(command => command.Description)
            .NotEmpty().WithMessage("Description is required.")
            .Length(10, 500).WithMessage("Description must be between 10 and 500 characters.");

        RuleFor(command => command.ProductId)
            .NotEmpty().WithMessage("Product ID is required.")
            .Must(BeAValidGuid).WithMessage("Product ID must be a valid GUID.");

        RuleFor(command => command.Discount)
            .InclusiveBetween(0.01m, 1.00m).WithMessage("Discount must be between 0.01 (1%) and 1.00 (100%).");

        RuleFor(command => command.StartDate)
            .NotEmpty().WithMessage("Start date is required.")
            .Must(BeAValidDate).WithMessage("Start date must be a valid date.")
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now))
            .WithMessage("Start date must be today or in the future.")
            .LessThan(command => command.EndDate)
            .WithMessage("Start date must be before or equal to end date.");

        RuleFor(command => command.EndDate)
            .NotEmpty()
            .WithMessage("End date is required.")
            .Must(BeAValidDate)
            .WithMessage("End date must be a valid date.")
            .GreaterThanOrEqualTo(comnand => comnand.StartDate.AddDays(1))
            .WithMessage("Start date must be greater than start date by at least one day.");
    }

    private bool BeAValidGuid(Guid guid)
    {
        return guid != Guid.Empty;
    }

    private bool BeAValidDate(DateOnly date)
    {
        return !date.Equals(default);
    }
}
