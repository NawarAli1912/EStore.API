using Application.Common.DatabaseAbstraction;
using Domain.Orders;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Primitives;

namespace Application.Orders.List;
internal sealed class ListOrdersQueryHandler
    : IRequestHandler<ListOrdersQuery, Result<ListOrderResult>>
{
    private readonly IApplicationDbContext _context;

    public ListOrdersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ListOrderResult>> Handle(
        ListOrdersQuery request,
        CancellationToken cancellationToken)
    {
        IQueryable<Order> ordersQuery = _context
            .Orders
            .Include(o => o.LineItems)
            .Where(o => request.Filter.Status.Contains(o.Status))
            .Where(o => o.ModifiedAtUtc >= request.Filter.ModifiedFrom)
            .Where(o => o.ModifiedAtUtc <= request.Filter.ModifiedTo)
            .OrderBy(o => o.ModifiedAtUtc)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize);

        return new ListOrderResult(
            await ordersQuery.ToListAsync(cancellationToken),
            await ordersQuery.CountAsync(cancellationToken));
    }
}
