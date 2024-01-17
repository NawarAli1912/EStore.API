using Application.Common.Cache;
using Application.Common.DatabaseAbstraction;
using Domain.Errors;
using Domain.Offers;
using Domain.Offers.Events;
using Domain.Products.Enums;
using MediatR;
using SharedKernel.Primitives;

namespace Application.Offers.CreateBundleDiscountOffer;
internal sealed class CreateBundleDiscountOfferCommandHandler : IRequestHandler<CreateBundleDiscountOfferCommand, Result<BundleDiscountOffer>>
{
    private readonly IApplicationDbContext _context;
    private readonly IProductsStore _productsStore;
    private readonly IOffersStore _offersStore;

    public CreateBundleDiscountOfferCommandHandler(
        IApplicationDbContext context,
        IProductsStore productsStore,
        IOffersStore offersStore)
    {
        _context = context;
        _productsStore = productsStore;
        _offersStore = offersStore;
    }

    public async Task<Result<BundleDiscountOffer>> Handle(CreateBundleDiscountOfferCommand request, CancellationToken cancellationToken)
    {
        List<Error> errors = [];

        var products = await _productsStore
            .GetByIds(request.Products.ToHashSet(), cancellationToken);

        if (products.Count != request.Products.Count)
        {
            return DomainError.Products.NotFound;
        }

        var productsOffers = products
            .SelectMany(p => p.AssociatedOffers)
            .ToHashSet();

        var percentageOffers = (await _offersStore.List())!
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
