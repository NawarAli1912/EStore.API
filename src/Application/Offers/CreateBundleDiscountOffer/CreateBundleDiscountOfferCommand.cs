using Domain.Offers;
using SharedKernel.Messaging;
using SharedKernel.Primitives;

namespace Application.Offers.CreateBundleDiscountOffer;
public record CreateBundleDiscountOfferCommand(
    string Name,
    string Description,
    List<Guid> Products,
    decimal Discount,
    DateOnly StartDate,
    DateOnly EndDate) : IRequest<Result<BundleDiscountOffer>>;
