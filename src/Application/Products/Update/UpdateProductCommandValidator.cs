using FluentValidation;

namespace Application.Products.Update;
public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty().WithMessage("Product ID is required.")
            .Must(BeAValidGuid).WithMessage("Product ID must be a valid GUID.");

        When(command => command.Name is not null, () =>
        {
            RuleFor(command => command.Name)
                .NotEmpty()
                .WithMessage("Name cannot be empty.")
                .Length(2, 100)
                .WithMessage("Name must be between 2 and 100 characters.");
        });

        When(command => command.Description is not null, () =>
        {
            RuleFor(command => command.Description)
                .NotEmpty()
                .WithMessage("Description cannot be empty.")
                .Length(10, 500)
                .WithMessage("Description must be between 10 and 500 characters.");
        });

        When(command => command.Quantity.HasValue, () =>
        {
            RuleFor(command => command.Quantity!.Value)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than 0.");
        });

        When(command => command.PurchasePrice.HasValue, () =>
        {
            RuleFor(command => command.PurchasePrice!.Value)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Purchase price must be greater than or equal to 0.");
        });

        When(command => command.CustomerPrice.HasValue, () =>
        {
            RuleFor(command => command.CustomerPrice!.Value)
                .GreaterThanOrEqualTo(0).WithMessage("Customer price must be greater than or equal to 0.");
        });
    }

    private bool BeAValidGuid(Guid guid)
    {
        return guid != Guid.Empty;
    }
}