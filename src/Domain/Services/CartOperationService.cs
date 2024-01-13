using Domain.Customers;
using Domain.Errors;
using Domain.Offers;
using Domain.Offers.Enums;
using Domain.Products;
using Domain.Products.Enums;
using Domain.Services.OffersPricingStartegy;
using SharedKernel.Enums;
using SharedKernel.Primitives;

namespace Domain.Services;

public class CartOperationService
{
    private readonly Customer _customer;

    public CartOperationService(Customer customer)
    {
        _customer = customer;
    }

    public Result<decimal> AddOfferItem(
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

            var inactiveProducts = offerProducts
                .Where(p => p.Status != ProductStatus.Active);
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

        var pricingStrategy = OfferProductsPricingStrategyFactory
            .GetStrategy(
                offer,
                offerProducts.ToDictionary(item => item.Id, item => item));
        var productToPrice = pricingStrategy.ComputeProductsPrices();

        var result = _customer.AddCartItem(offer.Id, requestedQuantity, ItemType.Offer);

        return result.IsError ?
            result.Errors :
            offer.CalculatePrice(productToPrice) * requestedQuantity;
    }

    public Result<decimal> RemoveOfferItem(
        Offer offer,
        List<Product> offerProducts,
        int requestedQuantity)
    {
        var result = _customer
            .RemoveCartItem(offer.Id, requestedQuantity, ItemType.Offer);

        if (result.IsError)
        {
            return result.Errors;
        }

        var pricingStrategy = OfferProductsPricingStrategyFactory
            .GetStrategy(
                offer,
                offerProducts.ToDictionary(item => item.Id, item => item));
        var productToPrice = pricingStrategy.ComputeProductsPrices();

        return result.IsError ?
           result.Errors :
           offer.CalculatePrice(productToPrice) * requestedQuantity;
    }

    public Result<decimal> AddProductItem(
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

        var result = _customer.AddCartItem(product.Id, requestedQuantity);

        if (result.IsError)
        {
            return result.Errors;
        }

        return product.CustomerPrice * requestedQuantity;
    }

    public Result<decimal> RemoveProductItem(
        Product product,
        int requestedQuantity)
    {
        var result = _customer.RemoveCartItem(product.Id, requestedQuantity);

        if (result.IsError)
        {
            return result.Errors;
        }

        return -product.CustomerPrice * requestedQuantity;
    }
}