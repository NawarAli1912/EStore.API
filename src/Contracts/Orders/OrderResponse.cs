using Contracts.Common;

namespace Contracts.Orders;

public record OrderResponse(
    Guid Id,
    Guid CustomerId,
    string Code,
    ShippingInfoResponse ShippingInfo,
    OrderStatus Status,
    DateTime CreatedAt,
    DateTime ModifiedAt,
    decimal TotalPrice,
    List<OrderItemResponse> LineItems
    );

public record ShippingInfoResponse(
    ShippingCompany ShippingCompany,
    string ShippingComapnyLocation,
    string PhoneNumber);

public record OrderItemResponse(
    Guid ProductId,
    int Quantity,
    decimal Price);
