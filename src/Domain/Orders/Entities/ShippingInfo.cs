using SharedKernel.Enums;
using SharedKernel.Primitives;

namespace Domain.Orders.Entities;
public class ShippingInfo : ValueObject
{
    public ShippingCompany ShippingCompany { get; private set; }

    public string ShippingCompanyLocation { get; private set; } = default!;

    public string PhoneNumber { get; private set; } = default!;

    public static ShippingInfo Create(
        ShippingCompany shippingCompany,
        string shippingCompanyLocation,
        string phoneNumber)
    {
        return new ShippingInfo
        {
            ShippingCompany = shippingCompany,
            ShippingCompanyLocation = shippingCompanyLocation,
            PhoneNumber = phoneNumber
        };
    }

    internal void Update(
        ShippingCompany? shippingCompany,
        string? shippingCompanyLocation,
        string? phoneNumber)
    {
        ShippingCompany = shippingCompany ?? ShippingCompany;
        ShippingCompanyLocation = shippingCompanyLocation ?? ShippingCompanyLocation;
        PhoneNumber = phoneNumber ?? PhoneNumber;
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return ShippingCompany;
        yield return ShippingCompanyLocation;
        yield return PhoneNumber;
    }
}
