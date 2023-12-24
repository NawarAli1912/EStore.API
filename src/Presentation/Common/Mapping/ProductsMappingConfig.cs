using Application.Products.AssignCategories;
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
        config.NewConfig<Product, CreateProductResponse>();

        config.NewConfig<Product, ProductDetailedResponse>();

        config.NewConfig<Product, ProductResponse>();

        config.NewConfig<Category, CategoryResponse>();

        config.NewConfig<(Guid, UpdateProductRequest), UpdateProductCommand>()
            .Map(dest => dest.Id, src => src.Item1)
            .Map(dest => dest, src => src.Item2);

        config.NewConfig<(Guid, AssignUnAssignCategoriesRequest), AssignCategoriesCommand>()
            .Map(dest => dest.Id, src => src.Item1)
            .Map(dest => dest, src => src.Item2);

        config.NewConfig<ListProductsFilter, ProductsFilter>()
            .MapWith(src => ListProductFilterToProductsFilter(src));

        config.NewConfig<ListProductsDetailsFilter, ProductsFilter>()
            .MapWith(src => ListProductsDetailsFilterToProductsFilter(src));
    }

    private ProductsFilter ListProductFilterToProductsFilter(ListProductsFilter src)
    {
        List<Domain.Products.Enums.ProductStatus> status =
            [Domain.Products.Enums.ProductStatus.Active];

        return new ProductsFilter(
                src.SearchTerm,
                src.MinPrice,
                src.MaxPrice,
                null,
                null,
                status
            );
    }

    private static ProductsFilter ListProductsDetailsFilterToProductsFilter(ListProductsDetailsFilter src)
    {
        List<Domain.Products.Enums.ProductStatus> status = [];

        if (src.Status is null || !src.Status.Any())
        {
            status = Enum
                    .GetValues(typeof(Domain.Products.Enums.ProductStatus))
                    .Cast<Domain.Products.Enums.ProductStatus>().ToList();
        }
        else
        {
            foreach (var s in src.Status)
            {
                status.Add(MapToDomainProductStatus(s));
            }
        }

        return new ProductsFilter(
                src.SearchTerm,
                src.MinPrice,
                src.MaxPrice,
                src.MinQuantity,
                src.MaxQuantity,
                status
            );
    }

    private static Domain.Products.Enums.ProductStatus MapToDomainProductStatus(ProductStatus src) => src switch
    {
        ProductStatus.Active => Domain.Products.Enums.ProductStatus.Active,
        ProductStatus.OutOfStock => Domain.Products.Enums.ProductStatus.OutOfStock,
        ProductStatus.Deleted => Domain.Products.Enums.ProductStatus.Deleted,

        _ => throw new ArgumentException()
    };
}

