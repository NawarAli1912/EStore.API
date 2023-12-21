using Domain.Customers;
using Domain.Orders.Entities;
using Domain.Products;
using Domain.Products.Errors;
using Domain.Products.ValueObjects;
using Domain.Services;
using SharedKernel.Enums;

namespace Domain.UnitTests.Services;
public sealed class OrderOrchestratorServiceUnitTests
{
    private static readonly Customer customer = Customer
            .Create(Guid.NewGuid());

    private static readonly ShippingInfo shippingInfo = ShippingInfo
        .Create(ShippingCompany.Alkadmous, "Location1", "+963992465535");

    private static readonly Sku sku1 = Sku.Create("Sku1").Value!;
    private static readonly Sku sku2 = Sku.Create("Sku1").Value!;

    private static readonly Product product1 = Product.Create(
        Guid.NewGuid(),
        "Product1",
        "Description1",
        20,
        99,
        80,
        sku1).Value;

    private static readonly Product product2 = Product.Create(
       Guid.NewGuid(),
       "Product2",
       "Description2",
       12,
       199,
       150,
       sku2).Value;


    readonly Dictionary<Guid, Product> productDict = new()
            {
                { product1.Id, product1 },
                { product2.Id, product2 },
            };


    [Fact]
    public void CreateOrder_ValidData_OrderCreatedSuccessfully()
    {
        // Arrange
        customer.AddCartItem(product1.Id, 2);
        customer.AddCartItem(product2.Id, 12);
        var product1InitialQuantity = product1.Quantity;
        var orderExpectedTotalPrice
            = 2 * product1.CustomerPrice + 12 * product2.CustomerPrice;
        var product1ExpectedQuantity = product1.Quantity - 2;


        // Act
        var result = OrderOrchestratorService
            .CreateOrder(customer, productDict, shippingInfo);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Empty(customer.Cart.CartItems);
        Assert.Contains(product1.Id, result.Value.LineItems.Select(li => li.ProductId));
        Assert.Equal(12, result.Value.LineItems.Where(li => li.ProductId == product2.Id).Count());
        Assert.Equal(product1InitialQuantity - 2, product1.Quantity);
        Assert.Equal(Products.Enums.ProductStatus.OutOfStock, product2.Status);
        Assert.Equal(orderExpectedTotalPrice, result.Value.TotalPrice);
        Assert.Equal(0, customer.Cart.CartItems.Count);
        Assert.Equal(product1ExpectedQuantity, product1.Quantity);
    }

    [Fact]
    public void CreateOrder_DeletedProductOrOutOfStock_ReturnsError()
    {
        // Arrange
        product1.MarkAsDeleted();
        product2.DecreaseQuantity(product2.Quantity);

        customer.AddCartItem(product1.Id, 1);
        customer.AddCartItem(product2.Id, 2);

        // Act
        var result = OrderOrchestratorService
            .CreateOrder(customer, productDict, shippingInfo);

        // Assert
        Assert.Contains(DomainError.Product.Deleted(product1.Name), result.Errors);
        Assert.Contains(DomainError.Product.OutOfStock(product2.Name), result.Errors);
    }

    [Fact]
    public void CreateOrder_EmptyCart_ReturnsError()
    {
        // Arrange
        customer.ClearCart();

        // Act
        var result = OrderOrchestratorService
            .CreateOrder(customer, productDict, shippingInfo);

        // Assert
        Assert.Contains(Customers.Errors.DomainError.Cart.EmptyCart, result.Errors);
    }

    [Fact]
    public void CreateOrder_InvalidProduct_ReturnsError()
    {
        // Arrange
        customer.AddCartItem(Guid.NewGuid(), 1);

        // Act
        var result = OrderOrchestratorService
            .CreateOrder(customer, productDict, shippingInfo);

        // Assert
        Assert.Contains(DomainError.Product.NotFound, result.Errors);
    }

    [Fact]
    public void CreateOrder_InvalidQuantity_ReturnsError()
    {
        // Arrange
        customer.AddCartItem(product1.Id, product1.Quantity + 1);

        // Act
        var result = OrderOrchestratorService
            .CreateOrder(customer, productDict, shippingInfo);

        // Assert
        Assert.Contains(DomainError.Product.StockError(product1.Name), result.Errors);
    }

    [Fact]
    public void CreateOrder_MixedValidInvalidProducts_ReturnsErrors()
    {
        // Arrange
        customer.AddCartItem(product1.Id, 1);
        customer.AddCartItem(Guid.NewGuid(), 1);
        customer.AddCartItem(product2.Id, product2.Quantity + 1);

        // Act
        var result = OrderOrchestratorService.CreateOrder(customer, productDict, shippingInfo);

        // Assert
        Assert.Contains(DomainError.Product.NotFound, result.Errors);
        Assert.Contains(DomainError.Product.StockError(product2.Name), result.Errors);
    }
}
