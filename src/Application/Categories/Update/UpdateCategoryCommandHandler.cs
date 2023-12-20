using Application.Common.Data;
using Domain.Categories;
using Domain.Categories.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Categories.Update;

internal class UpdateCategoryCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateCategoryCommand, Result<Category>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<Category>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context
                .Categories
                .Include(c => c.SubCategories)
                .FirstOrDefaultAsync(
                    c => c.Id == request.Id,
                    cancellationToken);

        if (category is null)
        {
            return DomainError.Category.NotFound;
        }

        category.Update(
            request.Name,
            request.ParentCategoryId,
            request.NullParent);

        await _context.SaveChangesAsync(cancellationToken);

        return category;
    }
}
