namespace Contracts.Offers;

public record CreateBundleDiscountOfferRequest(
    string Name,
    string Description,
    List<Guid> Products,
    decimal Discount,
    DateOnly StartDate,
    DateOnly EndDate);
