namespace Application.Offers.CreateBundleDiscountOffer;
using FluentValidation;
using System;

public class CreateBundleDiscountOfferCommandValidator : AbstractValidator<CreateBundleDiscountOfferCommand>
{
    public CreateBundleDiscountOfferCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Length(2, 100).WithMessage("Name must be between 2 and 100 characters.");

        RuleFor(command => command.Description)
            .NotEmpty().WithMessage("Description is required.")
            .Length(10, 500).WithMessage("Description must be between 10 and 500 characters.");

        RuleFor(command => command.Products)
            .NotEmpty().WithMessage("At least one product must be specified.")
            .Must(products => products.Count > 0).WithMessage("Product list cannot be empty.")
            .ForEach(product => product.NotEmpty().WithMessage("Product ID cannot be empty."));

        RuleFor(command => command.StartDate)
            .NotEmpty().WithMessage("Start date is required.")
            .Must(BeAValidDate).WithMessage("Start date must be a valid date.")
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now))
            .WithMessage("Start date must be today or in the future.")
            .LessThan(command => command.EndDate)
            .WithMessage("Start date must be before or equal to end date.");

        RuleFor(command => command.EndDate)
            .NotEmpty().WithMessage("End date is required.")
            .Must(BeAValidDate).WithMessage("End date must be a valid date.")
            .GreaterThanOrEqualTo(comnand => comnand.StartDate.AddDays(1))
            .WithMessage("Start date must be greater than start date by at least one day.");


        RuleFor(command => command.Products)
            .NotEmpty()
            .WithMessage("At least two products must be specified.")
            .Must(products => products.Count >= 2)
            .WithMessage("Product list must contain at least two products.")
            .ForEach(product => product.NotEmpty()
            .WithMessage("Product ID cannot be empty."));

        RuleFor(command => command.Discount)
           .InclusiveBetween(0.01m, 0.99m)
           .WithMessage("Discount must be between 0.01 and 1.00.");
    }

    private bool BeAValidDate(DateOnly date)
    {
        return !date.Equals(default);
    }
}
