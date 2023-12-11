using Domain.Kernal;
using Domain.Kernal.Enums;
using MediatR;

namespace Application.Carts.Checkout;

public record CheckoutCommand(
    Guid CustomerId,
    ShippingCompany ShippingCompany,
    string ShippingComapnyLocation,
    string PhoneNumber
    ) : IRequest<Result<Created>>;
