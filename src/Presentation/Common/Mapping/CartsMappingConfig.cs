﻿using Application.Carts.AddCartItem;
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
        config.NewConfig<(Guid, AddCartItemRequest), AddCartItemCommand>()
            .Map(dest => dest.CustomerId, src => src.Item1)
            .Map(dest => dest, src => src.Item2);

        config.NewConfig<(Guid, RemoveCartItemRequest), RemoveCartItemCommand>()
            .Map(dest => dest.CustomerId, src => src.Item1)
            .Map(dest => dest, src => src.Item2);

        config.NewConfig<ShippingCompany, SharedKernel.Enums.ShippingCompany>()
            .MapWith(src => MapToDomainShippingCompany(src));

        config.NewConfig<(Guid, CheckoutRequest), CheckoutCommand>()
            .Map(dest => dest.CustomerId, src => src.Item1)
            .Map(dest => dest, src => src.Item2);
    }

    private static SharedKernel.Enums.ShippingCompany MapToDomainShippingCompany(ShippingCompany src) => src switch
    {
        ShippingCompany.Alkadmous => SharedKernel.Enums.ShippingCompany.Alkadmous,
        ShippingCompany.Alfouad => SharedKernel.Enums.ShippingCompany.Alfouad,
        ShippingCompany.RahawanCargo => SharedKernel.Enums.ShippingCompany.RahawanCargo,
        _ => throw new ArgumentException(nameof(src)),
    };
}
