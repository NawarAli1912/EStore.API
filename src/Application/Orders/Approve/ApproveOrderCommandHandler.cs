using Application.Common.Cache;
using Application.Common.DatabaseAbstraction;
using Domain.Errors;
using Domain.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Primitives;

namespace Application.Orders.Approve;

internal sealed class ApproveOrderCommandHandler
        : IRequestHandler<ApproveOrderCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context;
    private readonly IProductsStore _productsStore;

    public ApproveOrderCommandHandler(
        IApplicationDbContext context,
        IProductsStore productsStore)
    {
        _context = context;
        _productsStore = productsStore;
    }

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

        var requestedProducts = await _productsStore
            .GetByIds(productsIds, cancellationToken);

        if (productsIds.Count != requestedProducts.Count)
        {
            return DomainError.Products.NotFound;
        }

        var orderOrchestratorService = new OrderOrchestratorService(requestedProducts);

        var result = orderOrchestratorService.Approve(order);

        if (result.IsError)
        {
            return result.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Updated;
    }
}
