using Application.Common.DatabaseAbstraction;
using Domain.Errors;
using Domain.Orders.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Primitives;

namespace Application.Orders.Cancel;
internal sealed class CancelOrderCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CancelOrderCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<Updated>> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .Include(o => o.LineItems)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (order is null)
        {
            return DomainError.Order.NotFound;
        }

        if (order.Status != OrderStatus.Pending)
        {
            return DomainError.Order.InvalidStatus(order.Status);
        }

        order.Cancel();

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Updated;
    }
}
