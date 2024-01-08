using MediatR;
using SharedKernel.Enums;
using SharedKernel.Primitives;

namespace Application.Orders.Update;

public record UpdateOrderCommand(
    Guid Id,
    UpdateShippingInfo? ShippingInfo,
    List<ProductItem> DeleteProducts,
    List<ProductItem> AddProducts,
    List<OfferItem> AddOffers,
    List<OfferItem> DeleteOffers
    ) : IRequest<Result<Updated>>;

public record UpdateShippingInfo(
    ShippingCompany? ShippingCompany,
    string? ShippingComapnyLocation,
    string? PhoneNumber);


public record ProductItem(
    Guid ProductId,
    int Quantity);

public record OfferItem(
    Guid OfferId,
    int Quantity);
