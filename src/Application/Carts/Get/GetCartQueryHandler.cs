using Application.Common.Cache;
using Application.Common.DatabaseAbstraction;
using Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Enums;
using SharedKernel.Primitives;

namespace Application.Carts.Get;
internal sealed class GetCartQueryHandler :
    IRequestHandler<GetCartQuery, Result<CartResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly IOffersStore _offersStore;
    private readonly IProductsStore _productsStore;

    public GetCartQueryHandler(
        IApplicationDbContext context,
        IOffersStore offersStore,
        IProductsStore productsStore)
    {
        _context = context;
        _offersStore = offersStore;
        _productsStore = productsStore;
    }

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

        var allOffers = await _offersStore.List();
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

        var productToPriceDict = (await _productsStore
            .GetByIds(productIds, cancellationToken))
            .ToDictionary(
                p => p.Id,
                p => p.CustomerPrice);

        if (productIds.Count != productToPriceDict.Count)
        {
            return DomainError.Products.NotPresentOnTheDictionary;
        }

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
            if (!offersDict.TryGetValue(cartItem.ItemId, out offer) ||
                offer.Status != Domain.Offers.Enums.OfferStatus.Published)
            {
                return DomainError.Offers.NotPresentOnTheDictionary;
            }

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
