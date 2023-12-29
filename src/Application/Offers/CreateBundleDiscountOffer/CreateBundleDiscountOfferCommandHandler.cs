using Application.Common.Data;
using Domain.Offers;
using Domain.Offers.Enums;
using Domain.Offers.Errors;
using Domain.Products.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Primitives;

namespace Application.Offers.CreateBundleDiscountOffer;
internal sealed class CreateBundleDiscountOfferCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateBundleDiscountOfferCommand, Result<BundleDiscountOffer>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<BundleDiscountOffer>> Handle(CreateBundleDiscountOfferCommand request, CancellationToken cancellationToken)
    {
        // check if all the products are not under other offer
        List<Error> errors = [];
        if (await _context.Offers.Where(o => o.Type == OfferType.BundleDiscountOffer)
            .Cast<PercentageDiscountOffer>()
            .AnyAsync(po => request.Products.Contains(po.ProductId), cancellationToken))
        {
            errors.Add(DomainError.Offer.UnderAnotherOffer);
        }

        // check if all the products are active and currently exists
        if (!(await _context.Products
                      .CountAsync(p => request
                                   .Products
                                   .Contains(p.Id) &&
                                       p.Status == ProductStatus.Active, cancellationToken)
               == request.Products.Count))
        {
            errors.Add(DomainError.Offer.UnspportedProducts);
        }

        if (errors.Count > 0)
        {
            return errors;
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
