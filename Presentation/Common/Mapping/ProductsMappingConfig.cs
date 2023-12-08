using Application.Products.Filters;
using Application.Products.List;
using Application.Products.Update;
using Contracts.Products;
using Domain.Categories;
using Domain.Products;
using Mapster;

namespace Presentation.Common.Mapping;

public class ProductsMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Product, CreateProductResponse>()
            .Map(dest => dest.CustomerPrice, src => src.CustomerPrice.Value)
            .Map(dest => dest.Currency, src => src.CustomerPrice.Currency.ToString())
            .Map(dest => dest.Sku, src => src.Sku == null ? "" : src.Sku.Value);


        config.NewConfig<Product, ProductDetailedResponse>()
            .Map(dest => dest.CustomerPrice, src => src.CustomerPrice.Value)
            .Map(dest => dest.PurchasePrice, src => src.PurchasePrice.Value)
            .Map(dest => dest.Currency, src => src.CustomerPrice.Currency.ToString())
            .Map(dest => dest.Sku, src => src.Sku == null ? "" : src.Sku.Value);

        config.NewConfig<Product, ProductResponse>()
            .Map(dest => dest.CustomerPrice, src => src.CustomerPrice.Value)
            .Map(dest => dest.Currency, src => src.CustomerPrice.Currency.ToString());

        config.NewConfig<Category, CategoryResponse>();

        config.NewConfig<(Guid, UpdateProductRequest), UpdateProductCommand>()
            .Map(dest => dest.Id, src => src.Item1)
            .Map(dest => dest, src => src.Item2);

        config.NewConfig<ListProductsFilter, ProductsFilter>();

        config.NewConfig<ListProductsDetailsFilter, ProductsFilter>();
    }
}
