using Contracts.Common;

namespace Contracts.Orders;

public record OrderResponse(
    Guid Id,
    Guid CustomerId,
    OrderStatus Status,
    ShippingInfoResponse ShippingInfo,
    DateTime CreatedAt,
    DateTime ModifiedAt,
    decimal TotalPrice,
    List<LineItemResponse> LineItems
    );

public record ShippingInfoResponse(
    ShippingCompany ShippingCompany,
    string ShippingComapnyLocation,
    string PhoneNumber
    );

public record LineItemResponse(
    Guid ProductId,
    int Quantity,
    decimal Price
    );
