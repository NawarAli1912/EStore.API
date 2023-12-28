using Domain.Customers;
using Domain.Products;
using Domain.Products.Enums;
using Domain.Products.Errors;
using SharedKernel.Primitives;

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
            return CustomerErrors.Customer.NotFound;
        }

        if (product is null)
        {
            return DomainError.Product.NotFound;
        }

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

        var decreaseQuantityResult = product
            .DecreaseQuantity(requestedQuantity);
        if (decreaseQuantityResult.IsError)
        {
            return decreaseQuantityResult.Errors;
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
            return CustomerErrors.Customer.NotFound;
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
