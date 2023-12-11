using Contracts.Common;

namespace Contracts.Carts;

public record CheckoutRequest(
    ShippingCompany ShippingCompany,
    string ShippingComapnyLocation,
    string PhoneNumber
    );
