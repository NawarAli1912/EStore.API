using Domain.Customers;
using Domain.Errors;
using Domain.Offers;
using Domain.Offers.Enums;
using Domain.Products;
using Domain.Products.Enums;
using SharedKernel.Primitives;

namespace Domain.Services;

public static class CartOperationService
{
    public static Result<decimal> AddOfferItem(
        Customer customer,
        Offer offer,
        List<Product> offerProducts,
        int requestedQuantity)
    {
        List<Error> errors = [];
        if (offerProducts.Any(p => p.Status != ProductStatus.Active))
        {
            if (offer.Status != OfferStatus.Published)
            {
                return DomainError.Offers.InvalidState(offer.Name);
            }

            var inactiveProducts = offerProducts.Where(p => p.Status != ProductStatus.Active);
            foreach (var product in inactiveProducts)
            {
                errors.Add(product.Status switch
                {
                    ProductStatus.Deleted =>
                        DomainError.Products.Deleted(product.Name),
                    ProductStatus.OutOfStock =>
                        DomainError.Products.OutOfStock(product.Name),
                    _ => DomainError.Products.InvalidState(product.Name)
                });
            }
        }

        if (errors.Count > 0)
        {
            return errors;
        }

        var result = customer.AddCartItem(offer.Id, requestedQuantity);

        return result.IsError ?
            result.Errors :
            offer.CalculatePrice(offerProducts.ToDictionary(p => p.Id, p => p.CustomerPrice));
    }

    public static Result<decimal> AddProductItem(
        Customer customer,
        Product product,
        int requestedQuantity)
    {
        if (product.Status != ProductStatus.Active)
        {
            return product.Status switch
            {
                ProductStatus.Deleted =>
                    DomainError.Products.Deleted(product.Name),
                ProductStatus.OutOfStock =>
                    DomainError.Products.OutOfStock(product.Name),
                _ => DomainError.Products.InvalidState(product.Name)
            };
        }

        var result = customer.AddCartItem(product.Id, requestedQuantity);

        if (result.IsError)
        {
            return result.Errors;
        }

        return product.CustomerPrice * requestedQuantity;
    }

    public static Result<decimal> RemoveCartItem(
        Customer customer,
        Product product,
        int requestedQuantity)
    {
        var result = customer.RemoveCartItem(product.Id, requestedQuantity);

        if (result.IsError)
        {
            return result.Errors;
        }

        return -product.CustomerPrice * requestedQuantity;
    }
}
