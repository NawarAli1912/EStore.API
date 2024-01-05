using Application.Common.DatabaseAbstraction;
using Domain.Offers;
using Domain.Offers.Events;
using Domain.Products;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Offers.EventsHandlers;
public sealed class OfferExpiredDomainEventHandler(IApplicationDbContext context)
        : INotificationHandler<OfferExpiredDomainEvent>
{
    private readonly IApplicationDbContext _context = context;

    public async Task Handle(
        OfferExpiredDomainEvent notification,
        CancellationToken cancellationToken)
    {
        List<Product> relatedProducts = [];
        if (notification.ExpiredOffer is BundleDiscountOffer bundleOffer)
        {
            relatedProducts = await _context
                .Products
                .Where(p => bundleOffer.BundleProductsIds.Contains(p.Id))
                .ToListAsync(cancellationToken);
        }
        else if (notification.ExpiredOffer is PercentageDiscountOffer percentageOffer)
        {
            relatedProducts = await _context
                .Products
                .Where(p => p.Id == percentageOffer.ProductId)
                .ToListAsync(cancellationToken);
        }

        foreach (var product in relatedProducts)
        {
            product.UnassociateOffers([notification.ExpiredOffer.Id]);
        }

        _context.Products.UpdateRange(relatedProducts);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
