using MediatR;
using SharedKernel;
using SharedKernel.Enums;

namespace Application.Carts.Checkout;

public record CheckoutCommand(
    Guid CustomerId,
    ShippingCompany ShippingCompany,
    string ShippingCompanyLocation,
    string PhoneNumber
    ) : IRequest<Result<Created>>;
