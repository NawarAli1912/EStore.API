using Domain.Customers;
using Domain.Orders;
using Domain.Orders.Entities;
using Domain.Orders.Enums;
using Domain.Products;
using Domain.Products.Enums;
using SharedKernel.Primitives;
using DomainError = Domain.Products.Errors.DomainError;

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
        ShippingInfo shippingInfo
        )
    {
        var cartItems = customer.Cart.CartItems.ToList();
        if (customer.Cart.CartItems.Count == 0)
        {
            return Customers.Errors.DomainError.Cart.EmptyCart;
        }

        var order = Order.Create(
            customer,
            shippingInfo);

        List<Error> errors = [];
        foreach (var item in cartItems)
        {
            if (!productDict.TryGetValue(item.ProductId, out var product))
            {
                errors.Add(DomainError.Product.NotFound);
                continue;
            }

            if (product.Status != ProductStatus.Active)
            {
                errors.Add(product.Status switch
                {
                    ProductStatus.Deleted =>
                        DomainError.Product.Deleted(product.Name),
                    ProductStatus.OutOfStock =>
                        DomainError.Product.OutOfStock(product.Name),
                    _ =>
                        DomainError.Product.InvalidState(product.Name)
                });
            }

            var decreaseQuantityResult = product.DecreaseQuantity(item.Quantity);
            if (decreaseQuantityResult.IsError)
            {
                errors.AddRange(decreaseQuantityResult.Errors);
                continue;
            }

            order.AddItems(product, item.Quantity);
        }

        if (errors.Count > 0)
        {

            return errors;
        }

        customer.ClearCart();

        return order;
    }

    /// <summary>
    ///     Rejects an order, increasing product quantities and updating order status.
    /// </summary>
    /// <param name="order">
    ///     The order to be rejected.
    /// </param>
    /// <param name="productDict">
    ///     A dictionary containing product information (mapping product ID to product).
    /// </param>
    public static void Reject(
        Order order,
        Dictionary<Guid, Product> productDict)
    {
        var lineItemsGroups = order
            .LineItems
            .GroupBy(li => li.ProductId);

        foreach (var group in lineItemsGroups)
        {
            var product = productDict[group.Key];

            product.IncreaseQuantity(group.Count());
        }

        order.Reject();
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

        if (order.Status == OrderStatus.Rejected)
        {
            var lineItemsGroups = order
                .LineItems
                .GroupBy(li => li.ProductId);

            foreach (var group in lineItemsGroups)
            {
                var product = productDict[group.Key];

                if (product.Status != ProductStatus.Active)
                {
                    return product.Status switch
                    {
                        ProductStatus.Deleted =>
                            DomainError.Product.Deleted(product.Name),
                        ProductStatus.OutOfStock =>
                            DomainError.Product.OutOfStock(product.Name),
                        _ =>
                            DomainError.Product.InvalidState(product.Name)
                    };
                }

                var result = product.DecreaseQuantity(group.Count());

                if (!result.IsError)
                {
                    continue;
                }

                errors.AddRange(result.Errors);
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
        List<Error> errors = [];
        foreach (var productId in itemsToAddQuantities.Keys)
        {
            if (!productIdToProduct.TryGetValue(productId, out var product))
            {
                errors.Add(DomainError.Product.NotFound);
                continue;
            }

            var decreaseResult = product.DecreaseQuantity(itemsToAddQuantities[productId]);
            if (decreaseResult.IsError)
            {
                errors.Add(DomainError.Product.StockError(product.Name));
                continue;
            }

            order.AddItems(product, itemsToAddQuantities[productId]);
        }

        foreach (var productId in itemsToDeleteQuantities.Keys)
        {
            if (!productIdToProduct.TryGetValue(productId, out var product))
            {
                errors.Add(DomainError.Product.NotFound);
                continue;
            }

            product.IncreaseQuantity(itemsToDeleteQuantities[productId]);
            order.RemoveItems(product, itemsToDeleteQuantities[productId]);
        }

        if (errors.Count > 0)
        {
            return errors;
        }

        return Result.Updated;
    }
}
