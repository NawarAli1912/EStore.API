using Domain.Kernal;
using Domain.Kernal.Enums;
using Domain.Kernal.Models;

namespace Domain.Orders.Entities;
public class ShippingInfo : Entity<Guid>
{
    public Guid OrderId { get; private set; }

    public ShippingCompany ShippingCompany { get; private set; }

    public string ShippingComapnyLocation { get; private set; } = default!;

    public string PhoneNumber { get; private set; } = default!;

    private ShippingInfo(Guid id) : base(id)
    {
    }

    private ShippingInfo()
        : base(Guid.NewGuid())
    {
    }

    internal static Result<ShippingInfo> Create(
        Guid orderId,
        ShippingCompany shippingCompany,
        string shippingComapnyLocation,
        string phoneNumber)
    {
        return new ShippingInfo
        {
            OrderId = orderId,
            ShippingCompany = shippingCompany,
            ShippingComapnyLocation = shippingComapnyLocation,
            PhoneNumber = phoneNumber
        };
    }
}
