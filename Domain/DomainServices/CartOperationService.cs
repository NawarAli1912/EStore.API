using Domain.Customers;
using Domain.DomainErrors;
using Domain.Kernal;
using Domain.Products;

namespace Domain.DomainServices;
public sealed class CartOperationService
{
    public static Result<decimal> AddCartItem(
        Customer? customer,
        Product? product,
        int requestedQuantity)
    {
        if (customer is null)
        {
            return Errors.Customers.NotFound;
        }

        if (product is null)
        {
            return Errors.Product.NotFound;
        }

        if (product.Quantity < requestedQuantity)
        {
            return Errors.Product.StockError;
        }

        var result = customer.AddCartItem(product.Id, requestedQuantity);

        if (result.IsError)
        {
            return result.Errors;
        }

        return product.CustomerPrice.Value;
    }

    public static Result<decimal> RemoveCartItem(
        Customer? customer,
        Product? product,
        int requestedQuantity)
    {
        if (customer is null)
        {
            return Errors.Customers.NotFound;
        }

        if (product is null)
        {
            return Errors.Product.NotFound;
        }

        var result = customer.RemoveCartItem(product.Id, requestedQuantity);

        if (result.IsError)
        {
            return result.Errors;
        }

        return -product.CustomerPrice.Value;
    }
}
