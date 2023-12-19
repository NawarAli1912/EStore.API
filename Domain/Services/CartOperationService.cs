using Domain.Customers;
using Domain.Products;
using Domain.Products.Enums;
using Domain.Products.Errors;
using SharedKernel;

namespace Domain.Services;

using CustomerErrors = Customers.Errors.DomainError;

public static class CartOperationService
{
    public static Result<decimal> AddCartItem(
        Customer? customer,
        Product? product,
        int requestedQuantity)
    {
        if (customer is null)
        {
            return CustomerErrors.Customers.NotFound;
        }

        if (product is null)
        {
            return DomainError.Product.NotFound;
        }

        if (product.Status != ProductStatus.Active)
        {
            return DomainError.Product.Inactive(product.Name);
        }

        if (product.Quantity < requestedQuantity)
        {
            return DomainError.Product.StockError(product.Name);
        }

        var result = customer.AddCartItem(product.Id, requestedQuantity);

        if (result.IsError)
        {
            return result.Errors;
        }

        return product.CustomerPrice * requestedQuantity;
    }

    public static Result<decimal> RemoveCartItem(
        Customer? customer,
        Product? product,
        int requestedQuantity)
    {
        if (customer is null)
        {
            return CustomerErrors.Customers.NotFound;
        }

        if (product is null)
        {
            return DomainError.Product.NotFound;
        }

        var result = customer.RemoveCartItem(product.Id, requestedQuantity);

        if (result.IsError)
        {
            return result.Errors;
        }

        return -product.CustomerPrice * requestedQuantity;
    }
}
