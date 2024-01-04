using Application.Common.DatabaseAbstraction;
using Domain.Categories;
using Domain.Products.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Primitives;

namespace Application.Products.UnassignCategories;
internal sealed class UnassignCategoriesCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UnassignCategoriesCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<Updated>> Handle(UnassignCategoriesCommand request, CancellationToken cancellationToken)
    {
        var product = await _context
            .Products
            .Include(p => p.Categories)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product is null)
        {
            return DomainError.Product.NotFound;
        }

        var categoriesDict = product
            .Categories.Select(item => item)
            .ToDictionary(i => i.Id, i => i);

        List<Error> errors = [];
        List<Category> categories = [];
        foreach (var categoryId in request.CategoriesIds)
        {
            if (!categoriesDict.TryGetValue(categoryId, out var category))
            {
                errors.Add(DomainError.Product.UnassignedCategory(product.Name, categoryId));
                continue;
            }
            categories.Add(category);
        }

        if (errors.Count > 0)
        {
            return errors;
        }

        product.UnassignCategories(categories);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Updated;
    }
}
