namespace Contracts.Offers;
public sealed record ListOfferResponse(
    List<PercentageDiscountOfferResponse> PercentageDiscountOffers,
    List<BundleDiscountOfferRespone> BundleDiscountOffers);

public sealed record PercentageDiscountOfferResponse(
    Guid Id,
    string Name,
    string Description,
    Guid ProductId,
    decimal Discount,
    OfferStatus Status,
    DateOnly StartsAt,
    DateOnly EndAt,
    decimal Price);


public sealed record BundleDiscountOfferRespone(
    Guid Id,
    string Name,
    string Description,
    List<Guid> ProductIds,
    decimal Discount,
    OfferStatus Status,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal Price);

