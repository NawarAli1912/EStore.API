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
        RuleFor(item => item.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(100).WithMessage("Product name cannot exceed 100 characters.");

        RuleFor(item => item.Description)
            .NotEmpty().WithMessage("Product description is required.")
            .MaximumLength(500).WithMessage("Product description cannot exceed 500 characters.");

        RuleFor(item => item.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0.");

        RuleFor(item => item.CustomerPrice)
            .GreaterThan(0).WithMessage("Customer price must be greater than 0.");

        RuleFor(item => item.PurchasePrice)
            .GreaterThan(0).WithMessage("Purchase price must be greater than 0.")
            .LessThan(item => item.CustomerPrice)
                .WithMessage("Purchase price must be less than the customer price.");
    }
}
