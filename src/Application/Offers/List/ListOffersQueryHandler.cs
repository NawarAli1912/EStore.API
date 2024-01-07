using Application.Common.DatabaseAbstraction;
using Application.Common.Repository;
using Domain.Errors;
using Domain.Services.OffersPricingStartegy;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Primitives;

namespace Application.Offers.List;
internal sealed class ListOffersQueryHandler(
    IOffersRepository offersRepository,
    IApplicationDbContext context) : IRequestHandler<ListOffersQuery, Result<ListOfferResult>>
{
    private readonly IOffersRepository _offersRepository = offersRepository;
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<ListOfferResult>> Handle(
        ListOffersQuery request,
        CancellationToken cancellationToken)
    {
        var offers = await _offersRepository.List();
        if (offers is null)
        {
            return DomainError.Offers.CantLoad;
        }

        Dictionary<Guid, decimal> offersPrices = [];
        var productsIds = offers
            .SelectMany(o => o.ListRelatedProductsIds())
            .ToHashSet();

        var productsDict = await _context
            .Products
            .Where(p => productsIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, cancellationToken);

        foreach (var offer in offers)
        {
            var offerPricingStrategy = OfferPricingStrategyFactory
                .GetStrategy(offer);

            var offerPrice = offerPricingStrategy
                .Handle(offer, productsDict)
                .Sum(kv => kv.Value);

            offersPrices.Add(offer.Id, offerPrice);
        }

        return new ListOfferResult(offers, offersPrices);
    }
}
