using Application.Common.Data;
using Domain.Orders.Enums;
using Domain.Products.Errors;
using Domain.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Orders.Update;

internal sealed class UpdateOrderCommandHandler(IApplicationDbContext context)
        : IRequestHandler<UpdateOrderCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<Updated>> Handle(
        UpdateOrderCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _context
            .Orders
            .Include(o => o.LineItems)
            .Include(o => o.ShippingInfo)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (order is null)
        {
            return DomainError.Product.NotFound;
        }

        if (order.Status != OrderStatus.Pending)
        {
            return Domain.Orders.Errors.DomainError.Orders.InvalidStatus(order.Status);
        }

        var productsIds = order.LineItems.Select(o => o.ProductId)
            .Concat(request.AddLineItems.Select(item => item.ProductId))
            .Concat(request.DeleteLineItems.Select(item => item.ProductId))
            .ToHashSet();

        var productsDict = await _context
            .Products
            .Where(p => productsIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => p, cancellationToken);


        var updateResult = OrderOrchestratorService.UpdateItems(
            order,
            productsDict,
            request.AddLineItems
                .ToDictionary(i => i.ProductId, i => i.Quantity),
            request.DeleteLineItems
                .ToDictionary(i => i.ProductId, i => i.Quantity));

        if (updateResult.IsError)
        {
            return updateResult.Errors;
        }

        order.UpdateShippingInfo(
            request.ShippingInfo.ShippingCompany,
            request.ShippingInfo.ShippingComapnyLocation,
            request.ShippingInfo.PhoneNumber);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Updated;
    }
}
