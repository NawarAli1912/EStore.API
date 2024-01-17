using Application.Common.DatabaseAbstraction;
using Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Primitives;

namespace Application.Categories.Delete;
internal sealed class DeleteCategoryCommandHandler
    : IRequestHandler<DeleteCategoryCommand, Result<Deleted>>
{
    private readonly IApplicationDbContext _context;

    public DeleteCategoryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Deleted>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .Include(c => c.SubCategories)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (category is null)
        {
            return DomainError.Categories.NotFound;
        }

        category.PrepareDelete(request.Action);

        _context.Categories.Remove(category);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Deleted;
    }
}
