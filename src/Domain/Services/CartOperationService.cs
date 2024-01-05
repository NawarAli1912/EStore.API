using Domain.Customers;
using Domain.Errors;
using Domain.Products;
using Domain.Products.Enums;
using SharedKernel.Primitives;

namespace Domain.Services;

public static class CartOperationService
{
    public static Result<decimal> AddCartItem(
        Customer? customer,
        Product? product,
        int requestedQuantity)
    {
        if (customer is null)
        {
            return DomainError.Customers.NotFound;
        }

        if (product is null)
        {
            return DomainError.Products.NotFound;
        }

        if (product.Status != ProductStatus.Active)
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
            return DomainError.Customers.NotFound;
        }

        if (product is null)
        {
            return DomainError.Products.NotFound;
        }

        var result = customer.RemoveCartItem(product.Id, requestedQuantity);

        if (result.IsError)
        {
            return result.Errors;
        }

        return -product.CustomerPrice * requestedQuantity;
    }
}
