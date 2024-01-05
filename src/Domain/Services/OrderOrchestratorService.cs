using Domain.Customers;
using Domain.Errors;
using Domain.Offers;
using Domain.Orders;
using Domain.Products;
using Domain.Products.Enums;
using Domain.Services.OffersPricingStartegy;
using SharedKernel.Enums;
using SharedKernel.Primitives;

namespace Domain.Services;
public static class OrderOrchestratorService
{
    /// <summary>
    /// Processes a new order for a specified customer.
    /// This function updates the product quantities based on the customer's cart,
    /// integrates shipping information, and handles potential errors.
    /// It starts by validating the cart contents,
    /// then creates an order using customer and shipping details.
    /// The function assesses product availability,
    /// calculates prices (including for offers),
    /// and adds these items to the order.
    /// If errors arise during this process, such as with product availability,
    /// the function returns these errors. On successful completion,
    /// the customer's cart is cleared, and the order is returned.
    /// </summary>
    /// <param name="customer">
    ///     The customer for whom the order is being created.
    /// </param>
    /// <param name="productDict">
    ///     Dictionary mapping each product's ID to its corresponding product details.
    /// </param>
    /// <param name="offersDict">
    ///     Dictionary holding offer information, keyed by offer ID.
    /// </param>
    /// <param name="shippingCompany">
    ///     The shipping company to handle the order delivery.
    /// </param>
    /// <param name="shippingCompanyAddress">
    ///     The address of the shipping company.
    /// </param>
    /// <param name="phoneNumber">
    ///     Contact phone number for the order.
    /// </param>
    /// <returns>
    ///     A Result object containing the created order if the process is successful. In case of errors (e.g., empty cart, unavailable products), it returns a list of these errors.
    /// </returns>
    public static Result<Order> CreateOrder(
        Customer customer,
        Dictionary<Guid, Product> productDict,
        Dictionary<Guid, Offer> offersDict,
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

        var errors = ValidateProductsStatus(productDict);
        if (errors.Count > 0)
        {
            return errors;
        }

        foreach (var cartItem in cartItems)
        {
            if (cartItem.Type == ItemType.Product)
            {
                var product = productDict[cartItem.ItemId];

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

            var offer = offersDict[cartItem.ItemId];
            var pricingStartegy = OfferPricingStrategyFactory.GetStrategy(offer);
            var productToPrice = pricingStartegy.Handle(offer, productDict);


            foreach (var item in productToPrice)
            {
                var addItemResult = order.AddItems(
                    item.Key,
                    item.Value,
                    cartItem.Quantity,
                    ItemType.Offer,
                    offer.Id);

                if (addItemResult.IsError)
                {
                    errors.AddRange(addItemResult.Errors);
                }
            }

            order.AddRequestedOffer(cartItem.ItemId);
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
    public static Result<Updated> Approve(
        Order order,
        Dictionary<Guid, Product> productDict)
    {


        if (order.LineItems is null)
        {
            return DomainError.Orders.EmptyLineItems;
        }

        var errors = ValidateProductsStatus(productDict);
        if (errors.Count > 0)
        {
            return errors;
        }

        var lineItemsGroups = order
            .LineItems
            .GroupBy(li => li.ProductId);

        foreach (var group in lineItemsGroups)
        {
            var product = productDict[group.Key];

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
    public static Result<Updated> UpdateItems(
        Order order,
        Dictionary<Guid, Product> productIdToProduct,
        Dictionary<Guid, int> itemsToAddQuantities,
        Dictionary<Guid, int> itemsToDeleteQuantities)
    {
        if (order.Status == Orders.Enums.OrderStatus.Canceled)
        {
            return DomainError.Orders.InvalidStatus(order.Status);
        }

        List<Error> errors = [];
        foreach (var productId in itemsToAddQuantities.Keys)
        {
            if (!productIdToProduct.TryGetValue(productId, out var product))
            {
                errors.Add(DomainError.Products.NotFound);
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
            if (!productIdToProduct.TryGetValue(productId, out var product))
            {
                errors.Add(DomainError.Products.NotFound);
                continue;
            }

            product.IncreaseQuantity(itemsToDeleteQuantities[productId]);
            order.RemoveItems(product.Id, itemsToDeleteQuantities[productId]);
        }

        if (errors.Count > 0)
        {
            return errors;
        }

        return Result.Updated;
    }

    private static List<Error> ValidateProductsStatus(Dictionary<Guid, Product> productDict)
    {
        var errors = new List<Error>();
        foreach (var product in productDict.Values)
        {
            if (product.Status != ProductStatus.Active)
            {
                errors.Add(product.Status switch
                {
                    ProductStatus.Deleted =>
                        DomainError.Products.Deleted(product.Name),
                    ProductStatus.OutOfStock =>
                        DomainError.Products.OutOfStock(product.Name),
                    _ =>
                        DomainError.Products.InvalidState(product.Name)
                });
            }
        }

        return errors;
    }
}
