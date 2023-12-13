using Domain.Customers;
using Domain.DomainErrors;
using Domain.Kernal;
using Domain.Kernal.Enums;
using Domain.Orders;
using Domain.Orders.Enums;
using Domain.Products;
using Domain.Products.Enums;

namespace Domain.DomainServices;
public class OrderOrchestratorService
{
    public static Result<Order> CreateOrder(
        Customer customer,
        Dictionary<Guid, Product> productDict,
        ShippingCompany shippingCompany,
        string shippingComapnyAddress,
        string phoneNumber
        )
    {
        var order = Order.Create(
            customer,
            shippingCompany,
            shippingComapnyAddress,
            phoneNumber);

        List<Error> errors = [];
        var cartItems = customer.Cart.CartItems.ToList();
        foreach (var item in cartItems)
        {
            if (!productDict.TryGetValue(item.ProductId, out var product))
            {
                errors.Add(Errors.Product.NotFound);
                continue;
            }

            if (product.Status != ProductStatus.Active)
            {
                errors.Add(Errors.Product.Inactive(product.Name));
                continue;
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
                    errors.Add(Errors.Product.Inactive(product.Name));
                    continue;
                }

                var result = product.DecreaseQuantity(group.Count());

                if (result.IsError)
                {
                    errors.AddRange(result.Errors);
                    continue;
                }
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
            if (!productIdToProduct.TryGetValue(productId, out var product)
                || product is null)
            {
                errors.Add(Errors.Product.NotFound);
                continue;
            }

            var decreaseResult = product.DecreaseQuantity(itemsToAddQuantities[productId]);
            if (decreaseResult.IsError)
            {
                errors.Add(Errors.Product.StockError(product.Name));
                continue;
            }

            order.AddItems(product, itemsToAddQuantities[productId]);
        }

        foreach (var productId in itemsToDeleteQuantities.Keys)
        {
            if (!productIdToProduct.TryGetValue(productId, out var product)
                || product is null)
            {
                errors.Add(Errors.Product.NotFound);
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
