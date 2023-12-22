using MediatR;
using SharedKernel.Enums;
using SharedKernel.Primitives;

namespace Application.Orders.Update;

public record UpdateOrderCommand(
    Guid Id,
    UpdateShippingInfo ShippingInfo,
    List<LineItemUpdate> DeleteLineItems,
    List<LineItemUpdate> AddLineItems
    ) : IRequest<Result<Updated>>;

public record DeleteLineItem(Guid Id);

public record LineItemUpdate(
    Guid ProductId,
    int Quantity);

public record UpdateShippingInfo(
    ShippingCompany? ShippingCompany,
    string? ShippingComapnyLocation,
    string? PhoneNumber);
