using Application.Common.DatabaseAbstraction;
using Application.Common.Repository;
using Domain.Errors;
using Domain.Offers;
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
            return DomainError.Products.NotFound;
        }

        var productsOffers = products
            .SelectMany(p => p.AssociatedOffers)
            .ToHashSet();

        var percentageOffers = (await _offersRepository.List())!
            .Where(o => o.Type == Domain.Offers.Enums.OfferType.PercentageDiscountOffer)
            .ToList()
            .Cast<PercentageDiscountOffer>();

        if (percentageOffers!.Any(o => productsOffers.Contains(o.ProductId)))
        {
            return DomainError.Offers.UnderAnotherOffer;
        }

        if (products.Any(p => p.Status != ProductStatus.Active))
        {
            return DomainError.Offers.UnspportedProducts;
        }

        var offer = BundleDiscountOffer.Create(
            request.Name,
            request.Description,
            request.Products,
            request.Discount,
            request.StartDate,
            request.EndDate);

        offer.RaiseDomainEvent(new OfferCreatedDomainEvent(offer));

        foreach (var product in products)
        {
            product.AssociateOffers([offer.Id]);
            _context.Products.Update(product);
        }

        await _context.Offers.AddAsync(offer, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return offer;
    }
}
