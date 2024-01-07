using Domain.Offers;

namespace Application.Offers.List;
public sealed record ListOfferResult(
    List<Offer> Offers,
    Dictionary<Guid, decimal> OffersPriceDict);


