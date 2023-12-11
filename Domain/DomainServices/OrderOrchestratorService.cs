using Domain.Customers;
using Domain.DomainErrors;
using Domain.Kernal;
using Domain.Kernal.Enums;
using Domain.Orders;
using Domain.Products;

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
}
