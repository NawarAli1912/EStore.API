using Contracts.Common;

namespace Contracts.Orders;

public record UpdateOrderRequest(
    ShippingInfoRequest? ShippingInfo,
    List<LineItemRequest>? DeleteLineItems,
    List<LineItemRequest>? AddLineItems,
    List<OfferUpdate>? AddOffers,
    List<OfferUpdate>? DeleteOffers
    );

public record LineItemRequest(
    Guid ProductId,
    int Quantity);

public record ShippingInfoRequest(
    ShippingCompany? ShippingCompany,
    string? ShippingComapnyLocation,
    string? PhoneNumber
    );

public record OfferUpdate(
    Guid OfferId,
    int Quantity);
