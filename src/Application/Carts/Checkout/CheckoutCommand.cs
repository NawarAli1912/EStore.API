using MediatR;
using SharedKernel.Enums;
using SharedKernel.Primitives;

namespace Application.Carts.Checkout;

public record CheckoutCommand(
    Guid CustomerId,
    ShippingCompany ShippingCompany,
    string ShippingCompanyLocation,
    string PhoneNumber
    ) : IRequest<Result<Created>>;
