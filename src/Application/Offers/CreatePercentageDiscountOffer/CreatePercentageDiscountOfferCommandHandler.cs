using Application.Common.Data;
using Domain.Offers;
using Domain.Offers.Enums;
using Domain.Offers.Events;
using Domain.Products.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Primitives;
using DomainError = Domain.Offers.Errors.DomainError;

namespace Application.Offers.CreatePercentageDiscountOffer;

internal sealed class CreatePercentageDiscountOfferCommandHandler(IApplicationDbContext context)
        : IRequestHandler<CreatePercentageDiscountOfferCommand, Result<PercentageDiscountOffer>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<PercentageDiscountOffer>> Handle(
        CreatePercentageDiscountOfferCommand request,
        CancellationToken cancellationToken)
    {
        List<Error> errors = [];
        var product = await _context.Products.FindAsync(request.ProductId);

        if (product is null)
        {
            return Domain.Products.Errors.DomainError.Product.NotFound;
        }

        if (product.Status != ProductStatus.Active)
        {
            return Domain.Products.Errors.DomainError.Product.InvalidState(product.Name);
        }

        if (await _context.Offers.Where(o => o.Type == OfferType.PercentageDiscountOffer)
            .Cast<PercentageDiscountOffer>()
            .AnyAsync(po => po.ProductId == request.ProductId, cancellationToken))
        {
            return DomainError.Offer.UnderAnotherOffer;
        }

        if (await _context.Offers.Where(o => o.Type == OfferType.BundleDiscountOffer)
            .Cast<BundleDiscountOffer>()
            .AnyAsync(po => po.BundleProductsIds.Contains(request.ProductId), cancellationToken))
        {
            return DomainError.Offer.UnderAnotherOffer;
        }

        var offer = PercentageDiscountOffer.Create(
            request.Name,
            request.Description,
            request.ProductId,
            request.Discount,
            request.StartDate,
            request.EndDate);

        offer.RaiseDomainEvent(new OfferCreatedDominaEvent(offer));

        product.AssociateOffer(offer.Id);
        _context.Products.Update(product);

        await _context.Offers.AddAsync(offer, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return offer;
    }
}
