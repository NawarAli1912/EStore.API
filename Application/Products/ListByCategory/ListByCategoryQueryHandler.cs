using Application.Common.Data;
using Application.Common.Repository;
using Application.Products.List;
using Domain.Kernal;
using MediatR;

namespace Application.Products.ListByCategory;

public sealed class ListByCategoryQueryHandler(IProductsRepository productsRepository, ICategoriesRepository categoriesRepository, IApplicationDbContext context) :
    IRequestHandler<ListByCategoryQuery, Result<ListProductResult>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IProductsRepository _productsRepository = productsRepository;
    private readonly ICategoriesRepository _categoriesRepository = categoriesRepository;

    public async Task<Result<ListProductResult>> Handle(
        ListByCategoryQuery request,
        CancellationToken cancellationToken)
    {
        var categoryIds = await _categoriesRepository.GetCategoryIdsInHierarchy(request.CategoryId);

        /*var items = await _context.Products
            .Include(p => p.Categories)
            .Where(p => categoryIds.Intersect(p.Categories.Select(c => c.Id)).Any())
            .ToListAsync();*/

        var products = await _productsRepository.ListByCategories(categoryIds);

        var totalCount = await _productsRepository.GetProductCountByCategory(categoryIds);

        return new ListProductResult([.. products], totalCount);
    }
}
