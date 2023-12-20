using Contracts.Common;

namespace Contracts.Orders;

public record UpdateOrderRequest(
    ShippingInfoRequest ShippingInfo,
    List<LineItemRequest>? DeleteLineItems,
    List<LineItemRequest>? AddLineItems
    );

public record LineItemRequest(
    Guid ProductId,
    int Quantity);

public record ShippingInfoRequest(
    ShippingCompany? ShippingCompany,
    string? ShippingComapnyLocation,
    string? PhoneNumber
    );
