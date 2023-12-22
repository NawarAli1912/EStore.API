using Application.Common.Data;
using Domain.Categories;
using Domain.ModelsSnapshots;
using Domain.Products.Errors;
using Domain.Products.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Primitives;

namespace Application.Products.AssignCategories;

internal class AssignCategoriesCommandHandler(IApplicationDbContext context)
        : IRequestHandler<AssignCategoriesCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<Updated>> Handle(AssignCategoriesCommand request, CancellationToken cancellationToken)
    {
        var product = await _context
            .Products
            .Include(p => p.Categories)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product is null)
        {
            return DomainError.Product.NotFound;
        }

        var categoriesDict = await _context
            .Categories
            .Where(c => request.CategoriesIds.Contains(c.Id))
            .ToDictionaryAsync(i => i.Id, i => i, cancellationToken);

        List<Category> productCateogries = [];
        foreach (var categoryId in request.CategoriesIds)
        {
            if (categoriesDict.TryGetValue(categoryId, out var category))
            {
                productCateogries.Add(category);
            }
        }

        product.AssignCategories(productCateogries);

        product.RaiseDomainEvent(
            new ProductUpdatedDomainEvent(ProductSnapshot.Snapshot(product)));

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Updated;
    }
}
