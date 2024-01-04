using Application.Carts.Common;
using Application.Common.DatabaseAbstraction;
using Domain.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Primitives;

namespace Application.Carts.AddCartItem;
internal sealed class AddCartItemCommandHandler(IApplicationDbContext context)
    : IRequestHandler<AddCartItemCommand, Result<AddRemoveCartItemResult>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<AddRemoveCartItemResult>> Handle(
        AddCartItemCommand request,
        CancellationToken cancellationToken)
    {
        var customer = await _context
            .Customers
            .Include(c => c.Cart)
            .ThenInclude(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.Id == request.CustomerId, cancellationToken: cancellationToken);

        var product = await _context
            .Products
            .FindAsync(request.ProductId, cancellationToken);

        var result = CartOperationService.AddCartItem(customer, product, request.Quantity);

        if (result.IsError)
        {
            return result.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new AddRemoveCartItemResult(result.Value);
    }
}
