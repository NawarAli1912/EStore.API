using Application.Common.Data;
using Domain.DomainErrors;
using Domain.Kernal;
using Domain.Orders.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Products.Delete;
internal class DeleteProductCommandHandler(IApplicationDbContext context)
        : IRequestHandler<DeleteProductCommand, Result<Deleted>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<Deleted>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var result = await _context
            .Products
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (result is null)
            return Errors.Product.NotFound;

        var productsInOrder = await _context
            .Orders
            .Where(o => o.Status == OrderStatus.Pending)
            .AnyAsync(o => o.LineItems.Any(li => li.ProductId == request.Id), cancellationToken);

        if (productsInOrder)
        {
            return Errors.Product.InOrder;
        }

        result.MarkAsDeleted();

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Deleted;
    }
}
