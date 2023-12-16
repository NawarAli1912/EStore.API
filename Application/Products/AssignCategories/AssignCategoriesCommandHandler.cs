using Application.Common.Data;
using Domain.Products.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Products.AssignCategories;

internal class AssignCategoriesCommandHandler(IApplicationDbContext context)
        : IRequestHandler<AssignCategoriesCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<bool>> Handle(AssignCategoriesCommand request, CancellationToken cancellationToken)
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

        foreach (var categoryId in request.CategoriesIds)
        {
            if (categoriesDict.TryGetValue(categoryId, out var category))
            {
                product.AssignCategory(category);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
