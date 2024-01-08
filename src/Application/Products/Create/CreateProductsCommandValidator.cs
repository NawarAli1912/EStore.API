using FluentValidation;

namespace Application.Products.Create;
public sealed class CreateProductsCommandValidator : AbstractValidator<CreateProductsCommand>
{
    public CreateProductsCommandValidator()
    {
        RuleFor(command => command.Items)
            .NotEmpty().WithMessage("Products list cannot be empty.")
            .ForEach(item => item.SetValidator(new CreateProductItemsValidator()));
    }
}

public class CreateProductItemsValidator : AbstractValidator<CreateProductItems>
{
    public CreateProductItemsValidator()
    {
        RuleFor(command => command.Name)
                .NotEmpty()
                .WithMessage("Name cannot be empty.")
                .Length(2, 100)
                .WithMessage("Name must be between 2 and 100 characters.");

        RuleFor(command => command.Description)
                .NotEmpty()
                .WithMessage("Description cannot be empty.")
                .Length(10, 500)
                .WithMessage("Description must be between 10 and 500 characters.");

        RuleFor(command => command.Quantity)
               .GreaterThan(0)
               .WithMessage("Quantity must be greater than 0.");

        RuleFor(command => command.PurchasePrice)
                .GreaterThanOrEqualTo(0m)
                .WithMessage("Purchase price must be greater than or equal to 0.");

        RuleFor(command => command.CustomerPrice)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Customer price must be greater than or equal to 0.")
                .GreaterThanOrEqualTo(command => command.CustomerPrice)
                .WithMessage("Purchase price must be less than the customer price.");
    }
}
