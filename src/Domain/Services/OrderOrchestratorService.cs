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

    /// <summary>
    /// Initializes a new instance of the OrderOrchestratorService class, setting it up to process customer cart and orders.
    /// </summary>
    /// <param name="products">An IEnumerable of products currently in the customer's cart or order. This IEnumerable should contain all the products that the customer intends to order.</param>
    /// <param name="offers">An optional IEnumerable of offers applicable to the products in the customer's cart. This IEnumerable should include any relevant offers that the customer can use for their order.</param>
    /// <remarks>
    /// This constructor is designed to be used with the specific contents of a customer's cart. 
    /// It requires an accurate IEnumerable of products and any applicable offers from the cart to properly orchestrate the order process.
    /// </remarks>

    public OrderOrchestratorService(IEnumerable<Product> products, IEnumerable<Offer>? offers = null)
    {
        _productDict = products
            .Distinct()
            .ToDictionary(p => p.Id);
        _offersDict = offers?
            .Distinct()
            .ToDictionary(o => o.Id) ?? [];
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

        var errors = ValidateProducts(
            _productDict,
            cartItems
            .Where(ci => ci.Type == ItemType.Product)
            .Select(ci => ci.ItemId));

        if (errors.Count > 0)
        {
            return errors;
        }

        foreach (var cartItem in cartItems)
        {
            if (cartItem.Type == ItemType.Product)
            {
                var product = _productDict[cartItem.ItemId];
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

            var offerProductsValidationErrors = ValidateProducts(
                _productDict,
                offer.ListRelatedProductsIds());

            if (offerProductsValidationErrors.Count > 0)
            {
                errors.AddRange(offerProductsValidationErrors);
                continue;
            }

            var offerAdditionStrategy = OfferAdditionStrategyFactory.GetStrategy(
                offer,
                _productDict);

            offerAdditionStrategy
                .Handle(order, cartItem.Quantity);
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
        if (order.LineItems.Count == 0)
        {
            return DomainError.Orders.EmptyLineItems;
        }

        var lineItemsGroups = order
            .LineItems
            .GroupBy(li => li.ProductId);

        var errors = ValidateProducts(
            _productDict,
            lineItemsGroups.Select(g => g.Key));

        if (errors.Count > 0)
        {
            return errors;
        }

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
        foreach (var item in offerToAdd)
        {
            if (!_offersDict.TryGetValue(item.Key, out var offer))
            {
                return DomainError.Offers.NotPresentOnTheDictionary;
            }

            var offerProductsValidationErrors = ValidateProducts(
                _productDict,
                offer.ListRelatedProductsIds());

            if (offerProductsValidationErrors.Count > 0)
            {
                return offerProductsValidationErrors;
            }

            var offerAdditionStrategy =
                OfferAdditionStrategyFactory.GetStrategy(offer, _productDict);

            var additionResult = offerAdditionStrategy.Handle(order, item.Value);

            if (additionResult.IsError)
            {
                errors.AddRange(additionResult.Errors);
            }
        }

        foreach (var item in offersToDelete)
        {
            if (!_offersDict.TryGetValue(item.Key, out var offer))
            {
                return DomainError.Offers.NotPresentOnTheDictionary;
            }

            var offerProductsValidationErrors = ValidateProducts(
                _productDict,
                offer.ListRelatedProductsIds());

            if (offerProductsValidationErrors.Count > 0)
            {
                return offerProductsValidationErrors;
            }

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
    /// <param name="productsToAddQuantity">
    ///     A dictionary specifying the quantities of items to add (mapping product ID to quantity to be added).
    /// </param>
    /// <param name="productsToRemvoeQuantity">
    ///     A dictionary specifying the quantities of items to delete (mapping product ID to quantity to be deleted).
    /// </param>
    /// <returns>
    ///     A result indicating whether the update was successful (Result.Updated) or any errors that occurred during the update.
    ///     If errors occur, the result will contain a list of error details.
    /// </returns>
    public Result<Updated> UpdateProductItems(
        Order order,
        Dictionary<Guid, int> productsToAddQuantity,
        Dictionary<Guid, int> productsToRemvoeQuantity)
    {
        List<Error> errors = ValidateProducts(
            _productDict,
            productsToAddQuantity.Select(kv => kv.Key)
            .Concat(productsToRemvoeQuantity.Select(kv => kv.Key)));

        if (errors.Count > 0)
        {
            return errors;
        }

        foreach (var productId in productsToAddQuantity.Keys)
        {
            var product = _productDict[productId];

            var decreaseResult = product
                .DecreaseQuantity(productsToAddQuantity[productId]);
            if (decreaseResult.IsError)
            {
                errors.Add(DomainError.Products.StockError(product.Name));
                continue;
            }

            order.AddItems(
                product.Id,
                product.CustomerPrice,
                productsToAddQuantity[productId]);
        }

        foreach (var productId in productsToRemvoeQuantity.Keys)
        {
            var product = _productDict[productId];

            product.IncreaseQuantity(productsToRemvoeQuantity[productId]);

            var removalResult = order.RemoveItems(
                product.Id,
                productsToRemvoeQuantity[productId]);
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

    private List<Error> ValidateProducts(Dictionary<Guid, Product> productDict, IEnumerable<Guid> requiredProducts)
    {
        var errors = new List<Error>();
        foreach (var productId in requiredProducts)
        {
            if (!productDict.TryGetValue(productId, out var product))
            {
                return [DomainError.Products.NotPresentOnTheDictionary];
            }

            if (product.Status != ProductStatus.Active)
            {
                var error = product.Status switch
                {
                    ProductStatus.Deleted =>
                        DomainError.Products.Deleted(product.Name),
                    ProductStatus.OutOfStock =>
                        DomainError.Products.OutOfStock(product.Name),
                    _ =>
                        DomainError.Products.InvalidState(product.Name)
                };

                errors.Add(error);
            }
        }
        return errors;
    }
}
