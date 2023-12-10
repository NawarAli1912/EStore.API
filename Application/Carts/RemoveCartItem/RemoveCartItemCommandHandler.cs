using Application.Carts.Common;
using Application.Common.Data;
using Domain.DomainServices;
using Domain.Kernal;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Carts.RemoveCartItem;

internal class RemoveCartItemCommandHandler(IApplicationDbContext context)
    : IRequestHandler<RemoveCartItemCommand, Result<AddRemoveCartItemResult>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<AddRemoveCartItemResult>> Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
    {
        var customer = await _context
            .Customers
            .Include(c => c.Cart)
            .ThenInclude(c => c.CartItems)
            .FirstOrDefaultAsync(
                c => c.Id == request.CustomerId,
                cancellationToken: cancellationToken);

        var product = await _context
            .Products
            .FindAsync(request.ProductId, cancellationToken);

        var result = CartOperationService.RemoveCartItem(customer, product, request.Quantity);

        if (result.IsError)
        {
            return result.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new AddRemoveCartItemResult(result.Value);
    }
}
