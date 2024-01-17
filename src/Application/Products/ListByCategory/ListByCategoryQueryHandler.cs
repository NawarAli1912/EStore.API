using Application.Common.Repository;
using Application.Products.List;
using MediatR;
using SharedKernel.Primitives;

namespace Application.Products.ListByCategory;

public sealed class ListByCategoryQueryHandler
    : IRequestHandler<ListByCategoryQuery, Result<ListProductResult>>
{
    private readonly IProductsRepository _productsRepository;
    private readonly ICategoriesRepository _categoriesRepository;

    public ListByCategoryQueryHandler(
        IProductsRepository productsRepository,
        ICategoriesRepository categoriesRepository)
    {
        _productsRepository = productsRepository;
        _categoriesRepository = categoriesRepository;
    }

    public async Task<Result<ListProductResult>> Handle(
        ListByCategoryQuery request,
        CancellationToken cancellationToken)
    {
        var categoryIds = await _categoriesRepository
            .GetCategoryIdsInHierarchy(request.CategoryId);

        var result = await _productsRepository
            .ListByCategories(categoryIds, request.Page, request.PageSize);

        return new ListProductResult(result.Item1, result.Item2);
    }
}
