using Application.Common.Repository;
using Application.Products.List;
using Domain.Kernal;
using MediatR;

namespace Application.Products.ListByCategory;

public sealed class ListByCategoryQueryHandler(
            IProductsRepository productsRepository,
            ICategoriesRepository categoriesRepository)
    : IRequestHandler<ListByCategoryQuery, Result<ListProductResult>>
{
    private readonly IProductsRepository _productsRepository = productsRepository;
    private readonly ICategoriesRepository _categoriesRepository = categoriesRepository;

    public async Task<Result<ListProductResult>> Handle(
        ListByCategoryQuery request,
        CancellationToken cancellationToken)
    {
        var categoryIds = await _categoriesRepository.GetCategoryIdsInHierarchy(request.CategoryId);

        var products = await _productsRepository.ListByCategories(categoryIds, request.Page, request.PageSize);

        var totalCount = await _productsRepository.GetProductCountByCategory(categoryIds);

        return new ListProductResult([.. products], totalCount);
    }
}
