namespace Application.Orders.Update;
using FluentValidation;
using Microsoft.IdentityModel.Tokens;

public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty().WithMessage("Order ID is required.");

        RuleFor(command => command.ShippingInfo)
            .SetValidator(new UpdateShippingInfoValidator());

        RuleFor(command => command.DeleteLineItems)
            .Must(lineItems => lineItems == null || lineItems.All(item => item != null))
            .WithMessage("Delete line items list cannot contain null elements.");

        RuleForEach(command => command.DeleteLineItems)
            .SetValidator(new LineItemUpdateValidator());

        RuleFor(command => command.AddLineItems)
            .Must(lineItems => lineItems == null || lineItems.All(item => item != null))
            .WithMessage("Add line items list cannot contain null elements.");

        RuleForEach(command => command.AddLineItems)
            .SetValidator(new LineItemUpdateValidator());

        RuleFor(command => command.DeleteLineItems)
           .Must(BeUniqueByProductId)
           .WithMessage("Delete line items list must not contain duplicate products.");

        RuleFor(command => command.AddLineItems)
            .Must(BeUniqueByProductId)
            .WithMessage("Add line items list must not contain duplicate products.");

        RuleFor(command => command)
            .Must(command => NotOverlap(command.DeleteLineItems, command.AddLineItems))
            .WithMessage("Delete and Add line items lists must not contain overlapping products.");
    }

    private bool BeUniqueByProductId(List<LineItemUpdate> lineItems)
    {
        return lineItems.GroupBy(item => item.ProductId).All(g => g.Count() == 1);
    }

    private bool NotOverlap(List<LineItemUpdate> deleteItems, List<LineItemUpdate> addItems)
    {
        var deleteProductIds = deleteItems.Select(item => item.ProductId).ToHashSet();
        var addProductIds = addItems.Select(item => item.ProductId);

        return !addProductIds.Any(deleteProductIds.Contains);
    }
}

public class LineItemUpdateValidator : AbstractValidator<LineItemUpdate>
{
    public LineItemUpdateValidator()
    {
        RuleFor(item => item.ProductId)
            .NotEmpty().WithMessage("Product ID is required.");

        RuleFor(item => item.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
    }
}

public class UpdateShippingInfoValidator : AbstractValidator<UpdateShippingInfo>
{
    public UpdateShippingInfoValidator()
    {
        RuleFor(info => info.ShippingComapnyLocation)
            .MaximumLength(255).When(info => info.ShippingCompany.HasValue)
            .WithMessage("Shipping company location is required.");


        RuleFor(info => info.PhoneNumber)
            .Matches(@"^\+9639[5-9]\d{7}$")
            .When(info => !info.PhoneNumber.IsNullOrEmpty())
            .WithMessage("Invalid phone number.");
    }
}

