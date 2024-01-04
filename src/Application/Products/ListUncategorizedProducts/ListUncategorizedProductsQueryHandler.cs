using Application.Common.DatabaseAbstraction;
using Application.Products.List;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Primitives;

namespace Application.Products.ListUncategorizedProducts;
internal sealed class ListUncategorizedProductsQueryHandler(IApplicationDbContext context) :
    IRequestHandler<ListUncategorizedProductsQuery, Result<ListProductResult>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<ListProductResult>> Handle(
        ListUncategorizedProductsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Products
                .Where(p => p.Categories.Count() == 0)
                .OrderBy(p => p.Id)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize);


        return new ListProductResult(
            await query.ToListAsync(cancellationToken),
            await query.CountAsync(cancellationToken));
    }
}
