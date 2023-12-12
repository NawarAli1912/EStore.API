using Application.Carts.AddCartItem;
using Application.Carts.Checkout;
using Application.Carts.RemoveCartItem;
using Contracts.Carts;
using Contracts.Common;
using Mapster;

namespace Presentation.Common.Mapping;

public class CartsMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<(Guid, AddRemoveCartItemRequest), AddCartItemCommand>()
            .Map(dest => dest.CustomerId, src => src.Item1)
            .Map(dest => dest, src => src.Item2);

        config.NewConfig<(Guid, AddRemoveCartItemRequest), RemoveCartItemCommand>()
            .Map(dest => dest.CustomerId, src => src.Item1)
            .Map(dest => dest, src => src.Item2);

        config.NewConfig<ShippingCompany, Domain.Kernal.Enums.ShippingCompany>()
            .MapWith(src => MapToDomainShippingCompany(src));

        config.NewConfig<(Guid, CheckoutRequest), CheckoutCommand>()
            .Map(dest => dest.CustomerId, src => src.Item1)
            .Map(dest => dest, src => src.Item2);
    }

    private static Domain.Kernal.Enums.ShippingCompany MapToDomainShippingCompany(ShippingCompany src) => src switch
    {
        ShippingCompany.Alkadmous => Domain.Kernal.Enums.ShippingCompany.Alkadmous,
        ShippingCompany.Alfouad => Domain.Kernal.Enums.ShippingCompany.Alfouad,
        ShippingCompany.RahawanCargo => Domain.Kernal.Enums.ShippingCompany.RahawanCargo,
        _ => throw new ArgumentException(nameof(src)),
    };
}
