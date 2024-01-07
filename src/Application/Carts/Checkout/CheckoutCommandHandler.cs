using Application.Common.DatabaseAbstraction;
using Application.Common.FriendlyIdentifiers;
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
    IOffersRepository offersRepository,
    IFriendlyIdGenerator friendlyIdGenerator)
        : IRequestHandler<CheckoutCommand, Result<Created>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IOffersRepository _offersRepository = offersRepository;
    private readonly IFriendlyIdGenerator _friendlyIdGenerator = friendlyIdGenerator;


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

        var cartOffersDict = allOffers!
            .Where(o => cartOffersIds.Contains(o.Id))
            .ToDictionary(o => o.Id, o => o);

        var productIds = customer.Cart.CartItems
            .Where(c => c.Type == ItemType.Product)
            .Select(c => c.ItemId)
            .Concat(cartOffersDict.Values.SelectMany(o => o.ListRelatedProductsIds()))
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

        var orderOrchestratorService = new OrderOrchestratorService(productDict, cartOffersDict);

        var orderResult = orderOrchestratorService.CreateOrder(
            customer,
            request.ShippingCompany,
            request.ShippingCompanyLocation,
            request.PhoneNumber);

        if (orderResult.IsError)
        {
            return orderResult.Errors;
        }

        orderResult
            .Value
            .SetCode((await _friendlyIdGenerator.GenerateOrderFriendlyId(1))[0]);


        await _context.Orders.AddAsync(orderResult.Value, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Created;
    }
}
