using Application.Common.DatabaseAbstraction;
using Application.Common.Repository;
using Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Enums;
using SharedKernel.Primitives;

namespace Application.Carts.Get;
internal sealed class GetCartQueryHandler(IApplicationDbContext context, IOffersRepository offersRepository) :
    IRequestHandler<GetCartQuery, Result<CartResult>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IOffersRepository _offersRepository = offersRepository;

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
            return DomainError.Customers.NotFound;
        }

        var allOffers = await _offersRepository.List();
        var cartOffersIds = customer
            .Cart
            .CartItems
            .Where(c => c.Type == ItemType.Offer)
            .Select(c => c.ItemId)
            .ToHashSet();

        var offersDict = allOffers!
            .Where(o => cartOffersIds.Contains(o.Id))
            .ToDictionary(o => o.Id, o => o);

        var productIds = customer.Cart.CartItems
            .Where(c => c.Type == ItemType.Product)
            .Select(c => c.ItemId)
            .Union(offersDict.Values.SelectMany(o => o.ListRelatedProductsIds()))
            .ToHashSet();

        var productToPriceDict = await _context
            .Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(
                p => p.Id,
                p => p.CustomerPrice,
                cancellationToken);

        List<CartItemResult> items = [];
        var totalPrice = 0.0M;
        foreach (var cartItem in customer.Cart.CartItems)
        {
            decimal itemPrice = 0.0M;
            if (cartItem.Type == ItemType.Product)
            {
                itemPrice = productToPriceDict[cartItem.ItemId]! * cartItem.Quantity;

                items.Add(new CartItemResult(
                    cartItem.ItemId,
                    cartItem.Type,
                    cartItem.Quantity,
                    itemPrice));

                totalPrice += itemPrice;
                continue;
            }

            var offer = offersDict[cartItem.ItemId];

            itemPrice = offer.CalculatePrice(productToPriceDict) * cartItem.Quantity;

            items.Add(new CartItemResult(
                    cartItem.ItemId,
                    cartItem.Type,
                    cartItem.Quantity,
                    itemPrice));

            totalPrice += itemPrice;
        }

        return new CartResult(items, totalPrice);
    }
}
