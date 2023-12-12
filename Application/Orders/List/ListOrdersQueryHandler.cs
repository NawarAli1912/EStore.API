using Application.Common.Data;
using Domain.Kernal;
using Domain.Orders;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Orders.List;
internal class ListOrdersQueryHandler(IApplicationDbContext context)
    : IRequestHandler<ListOrdersQuery, Result<List<Order>>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<List<Order>>> Handle(ListOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _context
            .Orders
            .Include(o => o.ShippingInfo)
            .Include(o => o.LineItems)
            .Where(o => request.Filter.Status.Contains(o.Status))
            .Where(o => o.ModifiedAt >= request.Filter.ModifiedFrom)
            .Where(o => o.ModifiedAt < request.Filter.ModifiedTo)
            .OrderBy(o => o.ModifiedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return orders;
    }
}
