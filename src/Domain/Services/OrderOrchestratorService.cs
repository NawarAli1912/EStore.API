using Domain.Customers;
using Domain.Errors;
using Domain.Offers;
using Domain.Orders;
using Domain.Products;
using Domain.Products.Enums;
using Domain.Services.OfferAdditionStrategy;
using Domain.Services.OfferRemovalStrategy;
using SharedKernel.Enums;
using SharedKernel.Primitives;

namespace Domain.Services;
public class OrderOrchestratorService
{
    private readonly Dictionary<Guid, Product> _productDict;
    private readonly Dictionary<Guid, Offer> _offersDict;

    public OrderOrchestratorService(Dictionary<Guid, Product> productDict, Dictionary<Guid, Offer>? offersDict = null)
    {
        _productDict = productDict;
        _offersDict = offersDict ?? [];
    }

    /// <summary>
    /// Creates an order for a customer, processing items from their cart.
    /// This method first ensures that necessary data structures (like offersDict) are initialized.
    /// It then validates the customer's cart for any items.
    /// If valid, it proceeds to create an order and adds each cart item to the order,
    /// handling products and offers differently.
    /// The method validates product statuses and manages addition strategies for offers.
    /// It accumulates any errors encountered during this process. After successfully adding all items,
    /// it clears the customer's cart and returns the created order.
    /// In case of errors at any stage, appropriate error messages are returned.
    /// </summary>
    /// <param name="customer">The customer placing the order, containing their cart items.</param>
    /// <param name="shippingCompany">The shipping company to be used for delivering the order.</param>
    /// <param name="shippingCompanyAddress">The address of the shipping company.</param>
    /// <param name="phoneNumber">The contact phone number for the order.</param>
    /// <returns>Returns an Order object if successful, or an error message if any issues arise during order creation.</returns>
    public Result<Order> CreateOrder(
        Customer customer,
        ShippingCompany shippingCompany,
        string shippingCompanyAddress,
        string phoneNumber)
    {
        var cartItems = customer
            .Cart
            .CartItems
            .ToList();

        if (customer.Cart.CartItems.Count == 0)
        {
            return DomainError.Carts.EmptyCart;
        }

        var order = Order.Create(
            customer.Id,
            shippingCompany,
            shippingCompanyAddress,
            phoneNumber);

        var orderProductsIds = cartItems
            .Select(ci => ci.ItemId)
            .ToHashSet();
        var errors = ValidateProductsStatus(
            _productDict
                    .Where(kv => orderProductsIds
                    .Contains(kv.Key))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value));

        if (errors.Count > 0)
        {
            return errors;
        }

        foreach (var cartItem in cartItems)
        {
            if (cartItem.Type == ItemType.Product)
            {
                if (!_productDict.TryGetValue(cartItem.ItemId, out var product))
                {
                    return DomainError.Products.NotPresentOnTheDictionary;
                }

                var addItemResult = order.AddItems(
                    product.Id,
                    product.CustomerPrice,
                    cartItem.Quantity);

                if (addItemResult.IsError)
                {
                    errors.AddRange(addItemResult.Errors);
                }

                continue;
            }

            if (!_offersDict.TryGetValue(cartItem.ItemId, out var offer))
            {
                return DomainError.Offers.NotPresentOnTheDictionary;
            }

            var offerAdditionStrategy = OfferAdditionStrategyFactory
                .GetStrategy(offer, _productDict);

            offerAdditionStrategy.Handle(order, cartItem.Quantity);
        }

        if (errors.Count > 0)
        {
            return errors;
        }

        customer.ClearCart();

        return order;
    }

    /// <summary>
    /// Approves an order, decreasing product quantities and updating order status if the order is not rejected.
    /// </summary>
    /// <param name="order">
    ///     The order to be approved.
    /// </param>
    /// <param name="productDict">
    ///     A dictionary containing product information (mapping product ID to product).
    /// </param>
    /// <returns>
    ///     A Result indicating whether the approval was successful (Result.Updated).
    ///     If errors occur during approval, the Result will contain a list of error details.
    /// </returns>
    public Result<Updated> Approve(Order order)
    {
        if (order.LineItems is null)
        {
            return DomainError.Orders.EmptyLineItems;
        }

        var errors = ValidateProductsStatus(_productDict);
        if (errors.Count > 0)
        {
            return errors;
        }

        var lineItemsGroups = order
            .LineItems
            .GroupBy(li => li.ProductId);

        foreach (var group in lineItemsGroups)
        {
            var product = _productDict[group.Key];

            var decreaseQuantityResult = product.DecreaseQuantity(group.Count());

            if (decreaseQuantityResult.IsError)
            {
                errors.AddRange(decreaseQuantityResult.Errors);
            }
        }

        if (errors.Count > 0)
        {
            return errors;
        }

        order.Approve();

        return Result.Updated;
    }

    public Result<Updated> UpdateOffersItems(
        Order order,
        Dictionary<Guid, int> offerToAdd,
        Dictionary<Guid, int> offersToDelete)
    {
        List<Error> errors = [];
        foreach (var items in offerToAdd)
        {
            var offerAdditionStrategy =
                OfferAdditionStrategyFactory.GetStrategy(_offersDict[items.Key], _productDict);

            var additionResult = offerAdditionStrategy.Handle(order, items.Value);

            if (additionResult.IsError)
            {
                errors.AddRange(additionResult.Errors);
            }
        }

        foreach (var item in offersToDelete)
        {
            var offerRemovalStrategy =
                OfferRemovalStrategyFactory.GetStrategy(_offersDict[item.Key]);

            var removalResult = offerRemovalStrategy.Handle(order, item.Value);

            if (removalResult.IsError)
            {
                errors.AddRange(removalResult.Errors);
            }
        }

        if (errors.Count > 0)
        {
            return errors;
        }

        return Result.Updated;
    }

    /// <summary>
    /// Updates the items in the order based on the provided dictionaries.
    /// </summary>
    /// <param name="order">
    ///     The order to be updated.
    /// </param>
    /// <param name="productIdToProduct">
    ///     A dictionary containing product information (mapping product ID to product).
    /// </param>
    /// <param name="itemsToAddQuantities">
    ///     A dictionary specifying the quantities of items to add (mapping product ID to quantity to be added).
    /// </param>
    /// <param name="itemsToDeleteQuantities">
    ///     A dictionary specifying the quantities of items to delete (mapping product ID to quantity to be deleted).
    /// </param>
    /// <returns>
    ///     A result indicating whether the update was successful (Result.Updated) or any errors that occurred during the update.
    ///     If errors occur, the result will contain a list of error details.
    /// </returns>
    public Result<Updated> UpdateProductItems(
        Order order,
        Dictionary<Guid, int> itemsToAddQuantities,
        Dictionary<Guid, int> itemsToDeleteQuantities)
    {

        List<Error> errors = [];
        foreach (var productId in itemsToAddQuantities.Keys)
        {

            if (!_productDict.TryGetValue(productId, out var product))
            {
                errors.Add(DomainError.Products.NotFound);
                continue;
            }

            var productValidation = ValidateProductStatus(product);
            if (productValidation.IsError)
            {
                errors.AddRange(productValidation.Errors);
                continue;
            }

            var decreaseResult = product.DecreaseQuantity(itemsToAddQuantities[productId]);
            if (decreaseResult.IsError)
            {
                errors.Add(DomainError.Products.StockError(product.Name));
                continue;
            }

            order.AddItems(
                product.Id,
                product.CustomerPrice,
                itemsToAddQuantities[productId]);
        }

        foreach (var productId in itemsToDeleteQuantities.Keys)
        {
            if (!_productDict.TryGetValue(productId, out var product))
            {
                errors.Add(DomainError.Products.NotFound);
                continue;
            }

            product.IncreaseQuantity(itemsToDeleteQuantities[productId]);
            var removalResult = order.RemoveItems(product.Id, itemsToDeleteQuantities[productId]);
            if (removalResult.IsError)
            {
                errors.AddRange(removalResult.Errors);
            }

        }

        if (errors.Count > 0)
        {
            return errors;
        }

        return Result.Updated;
    }

    private List<Error> ValidateProductsStatus(Dictionary<Guid, Product> productDict)
    {
        var errors = new List<Error>();
        foreach (var product in productDict.Values)
        {
            if (product.Status != ProductStatus.Active)
            {
                var result = ValidateProductStatus(product);

                if (result.IsError)
                    errors.AddRange(result.Errors);
            }
        }

        return errors;
    }

    private Result<Success> ValidateProductStatus(Product product)
    {

        return product.Status != ProductStatus.Active ?
            product.Status switch
            {
                ProductStatus.Deleted =>
                    DomainError.Products.Deleted(product.Name),
                ProductStatus.OutOfStock =>
                    DomainError.Products.OutOfStock(product.Name),
                _ =>
                    DomainError.Products.InvalidState(product.Name)
            } : Result.Success;
    }
}
