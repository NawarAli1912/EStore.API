﻿using Application.Carts.Common;
using Application.Common.DatabaseAbstraction;
using Domain.Customers;
using Domain.Errors;
using Domain.Offers;
using Domain.Offers.Enums;
using Domain.Products;
using Domain.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Primitives;

namespace Application.Carts.AddCartItem;
internal sealed class AddCartItemCommandHandler
    : IRequestHandler<AddCartItemCommand, Result<AddRemoveCartItemResult>>
{
    private readonly IApplicationDbContext _context;
    private CartOperationService? _cartOperationService;

    public AddCartItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<AddRemoveCartItemResult>> Handle(
        AddCartItemCommand request,
        CancellationToken cancellationToken)
    {
        var customer = await _context
            .Customers
            .Include(c => c.Cart)
            .ThenInclude(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.Id == request.CustomerId, cancellationToken);

        if (customer is null)
        {
            return DomainError.Customers.NotFound;
        }

        _cartOperationService = new CartOperationService(customer);

        var result = await (request.OfferId.HasValue switch
        {
            false => AddProductItem(request, customer, cancellationToken),
            true => AddOfferItem(request, customer, cancellationToken)
        });


        if (result.IsError)
        {
            return result.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new AddRemoveCartItemResult(result.Value);
    }

    private async Task<Result<decimal>> AddProductItem(AddCartItemCommand request, Customer customer, CancellationToken cancellationToken)
    {
        var product = await _context
            .Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

        if (product is null)
        {
            return DomainError.Products.NotFound;
        }

        return _cartOperationService!
            .AddProductItem(product, request.Quantity);
    }

    private async Task<Result<decimal>> AddOfferItem(AddCartItemCommand request, Customer customer, CancellationToken cancellationToken)
    {
        var offer = await _context.Offers
                .FirstOrDefaultAsync(o => o.Id == request.OfferId, cancellationToken);

        if (offer is null)
        {
            return DomainError.Offers.NotFound;
        }

        IQueryable<Product> query = _context.Products;

        query = offer.Type switch
        {
            OfferType.PercentageDiscountOffer => query.Where(p => p.Id == ((PercentageDiscountOffer)offer).ProductId),
            OfferType.BundleDiscountOffer => query.Where(p => ((BundleDiscountOffer)offer).BundleProductsIds.Contains(p.Id)),
            _ => throw new NotImplementedException()
        };
        var offerProducts = await query.ToListAsync(cancellationToken);

        return _cartOperationService!
            .AddOfferItem(offer, offerProducts, request.Quantity);
    }
}
