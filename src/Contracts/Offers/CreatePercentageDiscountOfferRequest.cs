namespace Contracts.Offers;
public record CreatePercentageDiscountOfferRequest(
    string Name,
    string Description,
    Guid ProductId,
    decimal Discount,
    DateOnly StartDate,
    DateOnly EndDate);
