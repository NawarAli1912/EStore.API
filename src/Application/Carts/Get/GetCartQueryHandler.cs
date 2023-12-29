using Application.Common.Data;
using Domain.Customers.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Primitives;

namespace Application.Carts.Get;
internal sealed class GetCartQueryHandler(IApplicationDbContext context) :
    IRequestHandler<GetCartQuery, Result<CartResult>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<CartResult>> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        List<Error> errors = [];
        var customer = await _context
            .Customers
            .Include(c => c.Cart)
            .ThenInclude(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.Id == request.CustomerId, cancellationToken);

        if (customer is null)
        {
            return DomainError.Customer.NotFound;
        }
        var productsIds = customer
            .Cart
            .CartItems
            .Select(c => c.ProductId)
            .ToHashSet();

        var productPriceDict = await _context
            .Products
            .Where(p => productsIds.Contains(p.Id))
            .Select(p => new
            {
                Id = p.Id,
                CustomerPrice = p.CustomerPrice
            })
            .ToDictionaryAsync(
                p => p.Id,
                p => p.CustomerPrice,
                cancellationToken: cancellationToken);

        List<CartItemResult> items = [];
        var totalPrice = 0.0M;
        foreach (var item in customer.Cart.CartItems)
        {
            var itemPrice =
                productPriceDict[item.ProductId]! * item.Quantity;

            items.Add(new CartItemResult(
                    item.ProductId,
                    item.Quantity,
                    itemPrice));

            totalPrice += itemPrice;
        }

        return new CartResult(items, totalPrice);
    }
}
