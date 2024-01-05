using Application.Common.DatabaseAbstraction;
using Application.Common.Repository;
using Domain.Errors;
using Domain.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Enums;
using SharedKernel.Primitives;

namespace Application.Carts.Checkout;
internal sealed class CheckoutCommandHandler(
    IApplicationDbContext context,
    IOffersRepository offersRepository)
        : IRequestHandler<CheckoutCommand, Result<Created>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IOffersRepository _offersRepository = offersRepository;


    public async Task<Result<Created>> Handle(
        CheckoutCommand request,
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

        if (customer.Cart.CartItems.Count == 0)
        {
            return DomainError.Carts.EmptyCart;
        }

        var allOffers = await _offersRepository.List();
        var cartOffersIds = customer
            .Cart
            .CartItems
            .Where(c => c.Type == ItemType.Offer)
            .Select(c => c.ItemId)
            .ToHashSet();

        var cartOffers = allOffers!
            .Where(o => cartOffersIds.Contains(o.Id))
            .ToDictionary(o => o.Id, o => o);

        var productIds = customer.Cart.CartItems
            .Where(c => c.Type == ItemType.Product)
            .Select(c => c.ItemId)
            .Union(cartOffers.Values.SelectMany(o => o.ListRelatedProductsIds()))
            .ToHashSet();

        var productDict = await _context
            .Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(
                p => p.Id,
                p => p,
                cancellationToken);

        if (productDict.Count != productIds.Count)
        {
            return DomainError.Products.NotFound;
        }

        var orderResult = OrderOrchestratorService.CreateOrder(
            customer,
            productDict,
            cartOffers,
            request.ShippingCompany,
            request.ShippingCompanyLocation,
            request.PhoneNumber);

        if (orderResult.IsError)
        {
            return orderResult.Errors;
        }

        await _context.Orders.AddAsync(orderResult.Value, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Created;
    }
}
