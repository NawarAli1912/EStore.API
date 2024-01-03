using Application.Common.Data;
using Application.Common.Repository;
using Domain.Offers;
using Domain.Offers.Errors;
using Domain.Offers.Events;
using Domain.Products.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Primitives;

namespace Application.Offers.CreateBundleDiscountOffer;
internal sealed class CreateBundleDiscountOfferCommandHandler(
    IApplicationDbContext context,
    IOffersRepository offersRepository) : IRequestHandler<CreateBundleDiscountOfferCommand, Result<BundleDiscountOffer>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IOffersRepository _offersRepository = offersRepository;

    public async Task<Result<BundleDiscountOffer>> Handle(CreateBundleDiscountOfferCommand request, CancellationToken cancellationToken)
    {
        List<Error> errors = [];

        var products = await _context
            .Products
            .Where(p => request.Products.Contains(p.Id))
            .ToListAsync(cancellationToken);

        if (products.Count != request.Products.Count)
        {
            return Domain.Products.Errors.DomainError.Product.NotFound;
        }

        var productsOffers = products
            .SelectMany(p => p.AssociatedOffers)
            .ToHashSet();

        var percentageOffers = await
            _offersRepository
            .ListPercentageDiscountOffers();

        if (percentageOffers!.Any(o => productsOffers.Contains(o.ProductId)))
        {
            return DomainError.Offer.UnderAnotherOffer;
        }

        if (products.Any(p => p.Status != ProductStatus.Active))
        {
            return DomainError.Offer.UnspportedProducts;
        }

        var offer = BundleDiscountOffer.Create(
            request.Name,
            request.Description,
            request.Products,
            request.Discount,
            request.StartDate,
            request.EndDate);

        offer.RaiseDomainEvent(new OfferCreatedDominaEvent(offer));

        foreach (var product in products)
        {
            product.AssociateOffer(offer.Id);
            _context.Products.Update(product);
        }

        await _context.Offers.AddAsync(offer, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return offer;
    }
}
