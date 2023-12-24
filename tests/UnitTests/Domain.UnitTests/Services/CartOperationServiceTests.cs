using Domain.Customers;
using Domain.Products;
using Domain.Products.Errors;
using Domain.Services;

namespace Domain.UnitTests.Services;
public class CartOperationServiceTests
{
    [Fact]
    public void AddCartItem_ValidData_CorrectResult()
    {
        // Arrange
        var customer = Customer.Create(Guid.NewGuid());
        var product = Product.Create(
            Guid.NewGuid(),
            "TestProduct",
            "Description",
            10,
            100,
            80);

        var requestedQuantity = 2;

        // Act
        var result = CartOperationService.AddCartItem(customer, product, requestedQuantity);

        // Assert
        Assert.Equal(product.CustomerPrice * requestedQuantity, result.Value);
        Assert.Equal(requestedQuantity, customer.Cart.CartItems.Single().Quantity);
    }
    [Fact]
    public void AddCartItem_NullProduct_ReturnsError()
    {
        // Arrange
        var customer = Customer.Create(Guid.NewGuid());
        Product? product = null;
        var requestedQuantity = 2;

        // Act
        var result = CartOperationService.AddCartItem(customer, product, requestedQuantity);

        // Assert
        Assert.Contains(DomainError.Product.NotFound, result.Errors);
    }

    [Fact]
    public void AddCartItem_NullCustomer_ReturnsError()
    {
        // Arrange
        Customer? customer = null;
        var product = Product.Create(
            Guid.NewGuid(),
            "TestProduct",
            "Description",
            10,
            100,
            80);
        var requestedQuantity = 2;

        // Act
        var result = CartOperationService
            .AddCartItem(customer, product, requestedQuantity);

        // Assert
        Assert.Contains(Customers.Errors.DomainError.Customers.NotFound, result.Errors);
    }

    [Fact]
    public void AddCartItem_SameProduct_CombineQuantities()
    {
        // Arrange
        var customer = Customer.Create(Guid.NewGuid());
        var product = Product.Create(
            Guid.NewGuid(),
            "TestProduct",
            "Description",
            10,
            100,
            80);

        var initialQuantity = 3;
        var additionalQuantity = 2;

        // Act
        var firstAdditionResult = CartOperationService
            .AddCartItem(customer, product, initialQuantity);
        var secondAdditionResult = CartOperationService
            .AddCartItem(customer, product, additionalQuantity);

        // Check that the product is in the cart
        var cartItem = customer.Cart.CartItems
            .SingleOrDefault(ci => ci.ProductId == product.Id);

        Assert.NotNull(cartItem);

        // Check that the quantity is the sum of the initial and additional quantities
        Assert.Equal(initialQuantity + additionalQuantity, cartItem.Quantity);
    }

    [Fact]
    public void AddCartItem_InactiveProduct_ReturnsError()
    {
        // Arrange
        var customer = Customer.Create(Guid.NewGuid());
        var product = Product.Create(
            Guid.NewGuid(),
            "TestProduct",
            "Description",
            10,
            100,
            80);

        product.MarkAsDeleted(); // Mark product as deleted
        var requestedQuantity = 2;

        // Act
        var result = CartOperationService.AddCartItem(customer, product, requestedQuantity);

        // Assert
        Assert.Contains(DomainError.Product.Deleted(product.Name), result.Errors);
    }

    [Fact]
    public void RemoveCartItem_ValidData_CorrectResult()
    {
        // Arrange
        var customer = Customer.Create(Guid.NewGuid());
        var product = Product.Create(
            Guid.NewGuid(),
            "TestProduct",
            "Description",
            10,
            100,
            80);

        customer.AddCartItem(product.Id, 3); // Add 3 items to cart
        var requestedQuantity = 2;

        // Act
        var result = CartOperationService.RemoveCartItem(customer, product, requestedQuantity);

        // Assert
        Assert.Equal(-product.CustomerPrice * requestedQuantity, result.Value);
        Assert.Equal(1, customer.Cart.CartItems.Single().Quantity);
    }

    [Fact]
    public void RemoveCartItem_NullCustomer_ReturnsError()
    {
        // Arrange
        Customer? customer = null;
        var product = Product.Create(
            Guid.NewGuid(),
            "TestProduct",
            "Description",
            10,
            100,
            80);

        var requestedQuantity = 2;

        // Act
        var result = CartOperationService.RemoveCartItem(customer, product, requestedQuantity);

        // Assert
        Assert.Contains(Customers.Errors.DomainError.Customers.NotFound, result.Errors);
    }

    [Fact]
    public void RemoveCartItem_NullProduct_ReturnsError()
    {
        // Arrange
        var customer = Customer.Create(Guid.NewGuid());
        Product? product = null;
        var requestedQuantity = 2;

        // Act
        var result = CartOperationService.RemoveCartItem(customer, product, requestedQuantity);

        // Assert
        Assert.Contains(DomainError.Product.NotFound, result.Errors);
    }

    [Fact]
    public void RemoveCartItem_RemoveMoreItemsThanInCart_ReturnsError()
    {
        // Arrange
        var customer = Customer.Create(Guid.NewGuid());
        var product = Product.Create(Guid.NewGuid(), "TestProduct", "Description", 10, 100, 80);
        customer.AddCartItem(product.Id, 2); // Add 2 items to cart

        // Act
        var result = CartOperationService.RemoveCartItem(customer, product, 3);

        // Assert
        Assert.Contains(Customers.Errors.DomainError.CartItem.NegativeQuantity, result.Errors);
    }
}
