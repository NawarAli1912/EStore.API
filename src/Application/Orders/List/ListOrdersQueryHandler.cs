using Application.Common.Data;
using Domain.Orders;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Primitives;

namespace Application.Orders.List;
internal class ListOrdersQueryHandler(IApplicationDbContext context)
    : IRequestHandler<ListOrdersQuery, Result<ListOrderResult>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<ListOrderResult>> Handle(
        ListOrdersQuery request,
        CancellationToken cancellationToken)
    {
        IQueryable<Order> ordersQuery = _context
            .Orders
            .Include(o => o.ShippingInfo)
            .Include(o => o.LineItems)
            .Where(o => request.Filter.Status.Contains(o.Status))
            .Where(o => o.ModifiedAtUtc >= request.Filter.ModifiedFrom)
            .Where(o => o.ModifiedAtUtc <= request.Filter.ModifiedTo)
            .OrderBy(o => o.ModifiedAtUtc)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize);

        var orders = await ordersQuery.ToListAsync(cancellationToken);
        var totalCount = await ordersQuery.CountAsync(cancellationToken);

        return new ListOrderResult(orders, totalCount);
    }
}
