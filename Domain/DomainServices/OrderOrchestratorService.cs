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
        string shippingComapnyLocation,
        string phoneNumber
        )
    {
        var orderResult = Order.Create(
            customer,
            shippingCompany,
            shippingComapnyLocation,
            phoneNumber
            );

        if (orderResult.IsError)
        {
            return orderResult.Errors;
        }


        List<Error> errors = [];
        var order = orderResult.Value;
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

            if (item.Quantity > product.Quantity)
            {
                errors.Add(Errors.Product.StockError);
                continue;
            }

            product.DecreaseQuantity(item.Quantity);
            for (int i = 0; i < item.Quantity; i++)
            {
                order.AddItem(product);
            }
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
}
