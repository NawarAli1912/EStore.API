using Domain.Customers;
using Domain.Errors;
using Domain.Orders;
using Domain.Products;
using Domain.Products.Enums;
using SharedKernel.Enums;
using SharedKernel.Primitives;

namespace Domain.Services;
public static class OrderOrchestratorService
{
    /// <summary>
    /// Creates a new order for a customer, updating product quantities and handling errors.
    /// </summary>
    /// <param name="customer">
    ///     The customer placing the order.
    /// </param>
    /// <param name="productDict">
    ///     A dictionary containing product information (mapping product ID to product).
    /// </param>
    /// <param name="shippingInfo">
    ///     A shipping info value object to relate it to the order
    /// </param>
    /// <returns>
    ///     A Result containing the created order if successful.
    ///     If errors occur during the order creation, the Result will contain a list of error details.
    /// </returns>
    public static Result<Order> CreateOrder(
        Customer customer,
        Dictionary<Guid, Product> productDict,
        ShippingCompany shippingCompany,
        string shippingCompanyAddress,
        string phoneNumber)
    {
        var cartItems = customer.Cart.CartItems.ToList();
        if (customer.Cart.CartItems.Count == 0)
        {
            return DomainError.Carts.EmptyCart;
        }

        var order = Order.Create(
            customer.Id,
            shippingCompany,
            shippingCompanyAddress,
            phoneNumber);

        List<Error> errors = [];
        foreach (var item in cartItems)
        {
            if (!productDict.TryGetValue(item.ItemId, out var product))
            {
                errors.Add(DomainError.Products.NotFound);
                continue;
            }

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

            order.AddItems(product.Id, product.CustomerPrice, item.Quantity);
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
        List<Error> errors = [];

        if (order.LineItems is null)
        {
            return DomainError.Orders.EmptyLineItems;
        }

        var lineItemsGroups = order
            .LineItems
            .GroupBy(li => li.ProductId);

        foreach (var group in lineItemsGroups)
        {
            if (!productDict.TryGetValue(group.Key, out var product))
            {
                errors.Add(DomainError.Products.NotPresentOnTheDictionary);
                continue;
            }

            if (product!.Status != ProductStatus.Active)
            {
                return product.Status switch
                {
                    ProductStatus.Deleted =>
                        DomainError.Products.Deleted(product.Name),
                    ProductStatus.OutOfStock =>
                        DomainError.Products.OutOfStock(product.Name),
                    _ =>
                        DomainError.Products.InvalidState(product.Name)
                };
            }

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
}
