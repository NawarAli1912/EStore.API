using Application.Common.Cache;
using Application.Common.DatabaseAbstraction;
using Domain.Errors;
using Domain.Services.OffersPricingStartegy;
using MediatR;
using SharedKernel.Primitives;

namespace Application.Offers.List;
internal sealed class ListOffersQueryHandler
    : IRequestHandler<ListOffersQuery, Result<ListOfferResult>>
{
    private readonly IOffersStore _offersStore;
    private readonly IProductsStore _productsStore;
    private readonly IApplicationDbContext _context;

    public ListOffersQueryHandler(
        IOffersStore offersRepository,
        IProductsStore productsStore,
        IApplicationDbContext context
        )
    {
        _offersStore = offersRepository;
        _context = context;
        _productsStore = productsStore;
    }

    public async Task<Result<ListOfferResult>> Handle(
        ListOffersQuery request,
        CancellationToken cancellationToken)
    {
        var offers = await _offersStore.List();
        if (offers is null)
        {
            return DomainError.Offers.CantLoad;
        }

        Dictionary<Guid, decimal> offersPrices = [];
        var productsIds = offers
            .SelectMany(o => o.ListRelatedProductsIds())
            .ToHashSet();

        var productsDict = (await _productsStore
            .GetByIds(productsIds, cancellationToken))
            .ToDictionary(p => p.Id);

        foreach (var offer in offers)
        {
            var offerProductPricingStrategy = OfferProductsPricingStrategyFactory
                    .GetStrategy(offer, productsDict);

            var offerProductsPrices = offerProductPricingStrategy.ComputeProductsPrices();

            offersPrices.Add(offer.Id, offer.CalculatePrice(offerProductsPrices));
        }

        return new ListOfferResult(offers, offersPrices);
    }
}
