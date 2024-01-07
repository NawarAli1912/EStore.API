using Application.Common.DatabaseAbstraction;
using Domain.Errors;
using Domain.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Primitives;

namespace Application.Orders.Approve;

internal sealed class ApproveOrderCommandHandler(IApplicationDbContext context)
        : IRequestHandler<ApproveOrderCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<Updated>> Handle(
        ApproveOrderCommand request,
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

        var productsIds = order
            .LineItems
            .Select(li => li.ProductId)
            .ToHashSet();

        var productsDict = await _context
            .Products
            .Where(p => productsIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => p, cancellationToken);

        if (productsIds.Count != productsDict.Count)
        {
            return DomainError.Products.NotFound;
        }

        var orderOrchestratorService = new OrderOrchestratorService(productsDict);

        var result = orderOrchestratorService.Approve(order);

        if (result.IsError)
        {
            return result.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Updated;
    }
}
