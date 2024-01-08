using Application.Common.DatabaseAbstraction;
using Application.Common.Repository;
using Domain.Errors;
using Domain.Orders.Enums;
using Domain.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Primitives;

namespace Application.Orders.Update;

internal sealed class UpdateOrderCommandHandler(IApplicationDbContext context, IOffersRepository offersRepository)
        : IRequestHandler<UpdateOrderCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IOffersRepository _offersRepository = offersRepository;

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

        var originalLineItemsIds = order
            .LineItems.Select(li => li.Id).ToList();

        var allOffers = await _offersRepository.List();

        var relatedOffersIds =
            request.AddOffers.Select(item => item.OfferId)
                .Concat(request.DeleteOffers.Select(item => item.OfferId))
                .Concat(order.RequestedOffers)
                .ToHashSet();

        var relatedOffersDict = allOffers!
            .Where(o => relatedOffersIds.Contains(o.Id))
            .ToDictionary(o => o.Id, o => o);

        if (relatedOffersDict.Count != relatedOffersIds.Count)
        {
            return DomainError.Offers.NotFound;
        }

        var productsIds = order.LineItems.Select(o => o.ProductId)
            .Concat(request.AddProducts.Select(item => item.ProductId))
            .Concat(request.DeleteProducts.Select(item => item.ProductId))
            .Concat(relatedOffersDict.Values.SelectMany(offer => offer.ListRelatedProductsIds()))
            .ToHashSet();

        var productsDict = await _context
            .Products
            .Where(p => productsIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => p, cancellationToken);


        if (productsDict.Count != productsIds.Count)
        {
            return DomainError.Products.NotFound;
        }


        var OrderOrchestratorService =
            new OrderOrchestratorService(productsDict, relatedOffersDict);

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
