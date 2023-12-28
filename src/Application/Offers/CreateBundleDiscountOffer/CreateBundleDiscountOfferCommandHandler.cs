using Application.Common.Data;
using Domain.Offers;
using Domain.Offers.Enums;
using Domain.Offers.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Primitives;

namespace Application.Offers.CreateBundleDiscountOffer;
internal sealed class CreateBundleDiscountOfferCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateBundleDiscountOfferCommand, Result<BundleDiscountOffer>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<BundleDiscountOffer>> Handle(CreateBundleDiscountOfferCommand request, CancellationToken cancellationToken)
    {
        // Check if any product under offer 
        if (await _context.Offers.Where(o => o.Type == OfferType.BundleDiscountOffer)
            .Cast<PercentageDiscountOffer>()
            .AnyAsync(po => request.Products.Contains(po.ProductId), cancellationToken))
        {
            return DomainError.Offer.UnderAnotherOffer;
        }

        var offer = BundleDiscountOffer.Create(
            request.Name,
            request.Description,
            request.Products,
            request.Discount,
            request.StartDate,
            request.EndDate);

        await _context.Offers.AddAsync(offer, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return offer;
    }
}
