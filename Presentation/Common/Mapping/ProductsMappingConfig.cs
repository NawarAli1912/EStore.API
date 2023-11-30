using Contracts.Products;
using Domain.Products;
using Mapster;

namespace Presentation.Common.Mapping;

public class ProductsMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Product, CreateProductResponse>()
            .Map(dest => dest.Price, src => src.Price.Value)
            .Map(dest => dest.Currency, src => src.Price.Currency.ToString())
            .Map(dest => dest.Sku, src => src.Sku == null ? "" : src.Sku.Value);
    }
}
