namespace Application.Orders.Update;
using FluentValidation;

public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty()
            .WithMessage("Order ID is required.");

        When(command => command.ShippingInfo is not null, () =>
        {
            RuleFor(command => command.ShippingInfo!.ShippingCompany)
                .IsInEnum()
                .When(info => info.ShippingInfo!.ShippingCompany.HasValue)
                .WithMessage("Shipping company must be a valid enum value");

            RuleFor(command => command.ShippingInfo!.ShippingComapnyLocation)
                .NotEmpty()
                .When(info => info.ShippingInfo!.ShippingComapnyLocation is not null)
                .WithMessage("Shipping company location must not be empty");

            RuleFor(command => command.ShippingInfo!.PhoneNumber)
                .Matches(@"^\+9639[5-9]\d{7}$")
                .When(info => info.ShippingInfo!.PhoneNumber is not null)
                .WithMessage("Invalid phone number.");
        });

        When(command => command.AddProducts is not null, () =>
        {
            RuleForEach(command => command.AddProducts)
                 .SetValidator(new ProductItemValidator());

            RuleFor(command => command.AddProducts)
                .Must(BeUniqueProduct)
                .WithMessage("Product IDs in AddProducts must be unique");

        });

        When(command => command.DeleteProducts is not null, () =>
        {
            RuleForEach(command => command.DeleteProducts)
            .SetValidator(new ProductItemValidator());

            RuleFor(command => command.DeleteProducts)
            .Must(BeUniqueProduct)
            .WithMessage("Product IDs in DeleteProducts must be unique");
        });

        When(command => command.AddOffers is not null, () =>
        {
            RuleForEach(command => command.AddOffers)
                .SetValidator(new OfferItemValidator());

            RuleFor(command => command.AddOffers)
            .Must(BeUniqueOffer)
            .WithMessage("Offer IDs in AddOffers must be unique");
        });

        When(command => command.DeleteOffers is not null, () =>
        {
            RuleForEach(command => command.DeleteOffers)
                .SetValidator(new OfferItemValidator());

            RuleFor(command => command.DeleteOffers)
           .Must(BeUniqueOffer)
           .WithMessage("Offer IDs in DeleteOffers must be unique");

        });


        When(command => command.AddProducts is not null
            && command.DeleteProducts is not null, () =>
        {
            RuleFor(command => command)
            .Must(command => !HaveOverlappingProductIds(command.AddProducts, command.DeleteProducts))
            .WithMessage("Product IDs in AddProducts and DeleteProducts must not overlap");
        });

        When(command => command.AddOffers is not null
            && command.DeleteOffers is not null, () =>
        {
            RuleFor(command => command)
            .Must(command => !HaveOverlappingOfferIds(command.AddOffers, command.DeleteOffers))
            .WithMessage("Offer IDs in AddOffers and DeleteOffers must not overlap");
        });
    }

    private bool BeUniqueProduct(List<ProductItem> products)
    {
        var productIds = products.Select(p => p.ProductId).ToList();
        return productIds.Distinct().Count() == productIds.Count;
    }

    private bool BeUniqueOffer(List<OfferItem> offers)
    {
        var offerIds = offers.Select(o => o.OfferId).ToList();
        return offerIds.Distinct().Count() == offerIds.Count;
    }

    private bool HaveOverlappingProductIds(List<ProductItem> addProducts, List<ProductItem> deleteProducts)
    {
        var addProductIds = new HashSet<Guid>(addProducts.Select(p => p.ProductId));
        return deleteProducts.Any(p => addProductIds.Contains(p.ProductId));
    }

    private bool HaveOverlappingOfferIds(List<OfferItem> addOffers, List<OfferItem> deleteOffers)
    {
        var addOfferIds = new HashSet<Guid>(addOffers.Select(o => o.OfferId));
        return deleteOffers.Any(o => addOfferIds.Contains(o.OfferId));
    }
}

public class ProductItemValidator : AbstractValidator<ProductItem>
{
    public ProductItemValidator()
    {
        RuleFor(product => product.ProductId)
            .NotEmpty()
            .WithMessage("Product ID must not be empty");

        RuleFor(product => product.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero");
    }
}

public class OfferItemValidator : AbstractValidator<OfferItem>
{
    public OfferItemValidator()
    {
        RuleFor(offer => offer.OfferId)
            .NotEmpty()
            .WithMessage("Offer ID must not be empty");

        RuleFor(offer => offer.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero");
    }
}

