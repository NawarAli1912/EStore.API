using Application.Carts.AddCartItem;
using Application.Carts.RemoveCartItem;
using Contracts.Carts;
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
    }
}
