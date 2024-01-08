using Contracts.Common;

namespace Contracts.Orders;

public record UpdateOrderRequest(
    ShippingInfoRequest? ShippingInfo,
    List<ProductItem>? DeleteProducts,
    List<ProductItem>? AddProducts,
    List<OfferItem>? AddOffers,
    List<OfferItem>? DeleteOffers
    );

public record ShippingInfoRequest(
    ShippingCompany? ShippingCompany,
    string? ShippingComapnyLocation,
    string? PhoneNumber
    );

public record ProductItem(
    Guid ProductId,
    int Quantity);


public record OfferItem(
    Guid OfferId,
    int Quantity);
