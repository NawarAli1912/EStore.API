using Application.Carts.Common;
using Application.Common.DatabaseAbstraction;
using Domain.Customers;
using Domain.Errors;
using Domain.Offers;
using Domain.Offers.Enums;
using Domain.Products;
using Domain.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Enums;
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

        var result = itemToRemove.Type switch
        {
            ItemType.Offer => await RemoveOfferItem(request, customer, cancellationToken),
            ItemType.Product => await RemoveProductItem(request, customer, cancellationToken),
            _ => throw new NotImplementedException()
        };

        if (result.IsError)
        {
            return result.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new AddRemoveCartItemResult(result.Value);
    }

    private async Task<Result<decimal>> RemoveProductItem(RemoveCartItemCommand request, Customer customer, CancellationToken cancellationToken)
    {
        var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == request.ItemId, cancellationToken);

        if (product is null)
        {
            return DomainError.Products.NotFound;
        }
        var result = CartOperationService.RemoveProductItem(customer, product, request.Quantity);

        return result;
    }

    private async Task<Result<decimal>> RemoveOfferItem(RemoveCartItemCommand request, Customer customer, CancellationToken cancellationToken)
    {
        var offer = await _context.Offers
               .Where(o => o.Id == request.ItemId)
               .FirstOrDefaultAsync(cancellationToken);

        if (offer is null)
        {
            return DomainError.Offers.NotFound;
        }

        IQueryable<Product> query = _context.Products;

        query = offer.Type switch
        {
            OfferType.PercentageDiscountOffer => query
                .Where(p => p.Id == ((PercentageDiscountOffer)offer).ProductId),
            OfferType.BundleDiscountOffer => query
                .Where(p => ((BundleDiscountOffer)offer).BundleProductsIds.Contains(p.Id)),
            _ => throw new NotImplementedException()
        };

        var offerProducts = await query.ToListAsync(cancellationToken);

        var result = CartOperationService
                    .RemoveOfferItem(customer, offer, offerProducts, request.Quantity);

        return result;
    }
}
