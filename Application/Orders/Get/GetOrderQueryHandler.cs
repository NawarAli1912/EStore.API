using Application.Common.Data;
using Domain.DomainErrors;
using Domain.Kernal;
using Domain.Orders;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Orders.Get;

internal sealed class GetOrderQueryHandler(IApplicationDbContext context) :
    IRequestHandler<GetOrderQuery, Result<Order>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<Order>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var order = await _context
            .Orders
            .Include(o => o.LineItems)
            .Include(o => o.ShippingInfo)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (order is null)
        {
            return Errors.Orders.NotFound;
        }

        return order;
    }
}
