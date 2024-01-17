using Application.Common.Cache;
using Application.Common.DatabaseAbstraction;
using Domain.Errors;
using Domain.Orders.Enums;
using Domain.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Primitives;

namespace Application.Orders.Update;

internal sealed class UpdateOrderCommandHandler
        : IRequestHandler<UpdateOrderCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context;
    private readonly IOffersStore _offersStore;
    private readonly IProductsStore _productsStore;

    public UpdateOrderCommandHandler(
        IApplicationDbContext context,
        IOffersStore offersStore,
        IProductsStore productsStore)
    {
        _context = context;
        _offersStore = offersStore;
        _productsStore = productsStore;
    }

    public async Task<Result<Updated>> Handle(
        UpdateOrderCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _context
            .Orders
            .Include(o => o.LineItems)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (order is null)
        {
            return DomainError.Products.NotFound;
        }

        if (order.Status != OrderStatus.Pending)
        {
            return DomainError.Orders.InvalidStatus(order.Status);
        }

        var allOffers = await _offersStore.List();

        var relatedOffersIds =
            request.AddOffers.Select(item => item.OfferId)
                .Concat(request.DeleteOffers.Select(item => item.OfferId))
                .Concat(order.RequestedOffers)
                .ToHashSet();

        var relatedOffers = allOffers!
            .Where(o => relatedOffersIds.Contains(o.Id))
            .ToList();

        if (relatedOffers.Count != relatedOffersIds.Count)
        {
            return DomainError.Offers.NotFound;
        }

        var productsIds = order.LineItems.Select(o => o.ProductId)
            .Concat(request.AddProducts.Select(item => item.ProductId))
            .Concat(request.DeleteProducts.Select(item => item.ProductId))
            .Concat(relatedOffers.SelectMany(offer => offer.ListRelatedProductsIds()))
            .ToHashSet();

        var requestedProducts = await _productsStore
            .GetByIds(productsIds, cancellationToken);


        if (requestedProducts.Count != productsIds.Count)
        {
            return DomainError.Products.NotFound;
        }


        var OrderOrchestratorService =
            new OrderOrchestratorService(requestedProducts, relatedOffers);

        var updateProductItemsResult = OrderOrchestratorService.UpdateProductItems(
            order,
            request.AddProducts
                .ToDictionary(i => i.ProductId, i => i.Quantity),
            request.DeleteProducts
                .ToDictionary(i => i.ProductId, i => i.Quantity));

        if (updateProductItemsResult.IsError)
        {
            return updateProductItemsResult.Errors;
        }

        var updateOfferItemsResult = OrderOrchestratorService.UpdateOffersItems(
            order,
            request.AddOffers
                .ToDictionary(i => i.OfferId, i => i.Quantity),
            request.DeleteOffers
                .ToDictionary(i => i.OfferId, i => i.Quantity));

        if (updateOfferItemsResult.IsError)
        {
            return updateOfferItemsResult.Errors;
        }

        if (request.ShippingInfo is not null)
        {
            order.UpdateShippingInfo(
                request.ShippingInfo.ShippingCompany,
                request.ShippingInfo.ShippingComapnyLocation,
                request.ShippingInfo.PhoneNumber);
        }

        _context.Orders.Update(order);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Updated;
    }
}
