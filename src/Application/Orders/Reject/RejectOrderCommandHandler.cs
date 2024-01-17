using Application.Common.DatabaseAbstraction;
using Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Primitives;

namespace Application.Orders.Reject;
internal sealed class RejectOrderCommandHandler
        : IRequestHandler<RejectOrderCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context;

    public RejectOrderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Updated>> Handle(
        RejectOrderCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _context
            .Orders
            .Include(o => o.LineItems)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (order is null)
        {
            return DomainError.Orders.NotFound;
        }

        order.Reject();

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Updated;
    }
}
