using Application.Common.Cache;
using Application.Common.DatabaseAbstraction;
using Application.Common.FriendlyIdentifiers;
using Domain.Errors;
using Domain.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Enums;
using SharedKernel.Primitives;

namespace Application.Carts.Checkout;
internal sealed class CheckoutCommandHandler
        : IRequestHandler<CheckoutCommand, Result<Created>>
{
    private readonly IApplicationDbContext _context;
    private readonly IOffersStore _offersStore;
    private readonly IProductsStore _productsStore;
    private readonly IFriendlyIdGenerator _friendlyIdGenerator;

    public CheckoutCommandHandler(
        IApplicationDbContext context,
        IOffersStore offersStore,
        IFriendlyIdGenerator friendlyIdGenerator,
        IProductsStore productsStore)
    {
        _context = context;
        _offersStore = offersStore;
        _friendlyIdGenerator = friendlyIdGenerator;
        _productsStore = productsStore;
    }

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

        var allOffers = await _offersStore.List();
        var cartOffersIds = customer
            .Cart
            .CartItems
            .Where(c => c.Type == ItemType.Offer)
            .Select(c => c.ItemId)
            .ToHashSet();

        var requestedOffers = allOffers!
            .Where(o => cartOffersIds.Contains(o.Id))
            .ToList();

        var productIds = customer.Cart.CartItems
            .Where(c => c.Type == ItemType.Product)
            .Select(c => c.ItemId)
            .Concat(requestedOffers.SelectMany(o => o.ListRelatedProductsIds()))
            .ToHashSet();

        var requestedProducts = await _productsStore
            .GetByIds(productIds, cancellationToken);

        if (requestedProducts.Count != productIds.Count)
        {
            return DomainError.Products.NotFound;
        }

        var orderOrchestratorService = new OrderOrchestratorService(requestedProducts, requestedOffers);

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
