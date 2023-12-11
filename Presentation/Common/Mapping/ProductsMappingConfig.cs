using Application.Products.AssignCategories;
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
            .Map(dest => dest.Sku, src => src.Sku == null ? "" : src.Sku.Value);


        config.NewConfig<Product, ProductDetailedResponse>()
            .Map(dest => dest.Sku, src => src.Sku == null ? "" : src.Sku.Value);

        config.NewConfig<Product, ProductResponse>();

        config.NewConfig<Category, CategoryResponse>();

        config.NewConfig<(Guid, UpdateProductRequest), UpdateProductCommand>()
            .Map(dest => dest.Id, src => src.Item1)
            .Map(dest => dest, src => src.Item2);

        config.NewConfig<(Guid, AssignCategoriesRequest), AssignCategoriesCommand>()
            .Map(dest => dest.Id, src => src.Item1)
            .Map(dest => dest, src => src.Item2);

        config.NewConfig<ListProductsFilter, ProductsFilter>();

        config.NewConfig<ListProductsDetailsFilter, ProductsFilter>();
    }
}
