using Application.Carts.Common;
using Application.Common.DatabaseAbstraction;
using Domain.Customers.Enums;
using Domain.Errors;
using Domain.Offers;
using Domain.Offers.Enums;
using Domain.Products;
using Domain.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Primitives;

namespace Application.Carts.RemoveCartItem;

internal sealed class RemoveCartItemCommandHandler(IApplicationDbContext context)
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
                cancellationToken);

        if (customer is null)
        {
            return DomainError.Customers.NotFound;
        }

        var itemToRemove = customer
            .Cart
            .CartItems
            .Where(ci => ci.ItemId == request.ItemId)
            .FirstOrDefault();

        if (itemToRemove is null)
        {
            return DomainError.CartItems.NotFound;
        }

        if (itemToRemove.Type == ItemType.Offer)
        {
            var offer = await _context.Offers
                .Where(o => o.Id == request.ItemId)
                .FirstOrDefaultAsync(cancellationToken);

            if (offer is null)
            {
                return DomainError.Offers.NotFound;
            }

            var offerProducts = await LoadProducts(offer);

            var removeOfferResult = CartOperationService
                .RemoveOfferItem(customer, offer, offerProducts, request.Quantity);

            if (removeOfferResult.IsError)
            {
                return removeOfferResult.Errors;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return new AddRemoveCartItemResult(removeOfferResult.Value);

        }

        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.ItemId, cancellationToken);

        if (product is null)
        {
            return DomainError.Products.NotFound;
        }

        var result = CartOperationService.RemoveProductItem(customer, product, request.Quantity);

        if (result.IsError)
        {
            return result.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new AddRemoveCartItemResult(result.Value);
    }

    private async Task<List<Product>> LoadProducts(Offer offer)
    {
        IQueryable<Product> query = _context.Products;

        query = offer.Type switch
        {
            OfferType.PercentageDiscountOffer => query.Where(p => p.Id == ((PercentageDiscountOffer)offer).ProductId),
            OfferType.BundleDiscountOffer => query.Where(p => ((BundleDiscountOffer)offer).BundleProductsIds.Contains(p.Id)),
            _ => throw new NotImplementedException()
        };

        return await query.ToListAsync();
    }
}
