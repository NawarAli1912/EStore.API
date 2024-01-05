using Domain.Customers;
using Domain.Errors;
using Domain.Orders.ValueObjects;
using Domain.Products;
using Domain.Services;

namespace Domain.UnitTests.Services;
public class CartOperationServiceTests
{
    private readonly Customer customer;
    private readonly ShippingInfo shippingInfo;
    private readonly Dictionary<Guid, Product> productDict;
    private readonly Product product1;
    private readonly Product product2;

    public CartOperationServiceTests()
    {
        customer = TestDataFactory.CreateCustomer();

        product1 = TestDataFactory.CreateProduct();
        product2 = TestDataFactory.CreateProduct();

        productDict = new Dictionary<Guid, Product>
        {
            { product1.Id, product1 },
            { product2.Id, product2 }
        };
    }

    [Fact]
    public void AddCartItem_ValidData_CorrectResult()
    {
        // Arrange
        var requestedQuantity = 2;

        // Act
        var result = CartOperationService.AddCartItem(
            customer,
            product1,
            requestedQuantity);

        // Assert
        Assert.Equal(product1.CustomerPrice * requestedQuantity, result.Value);
        Assert.Equal(requestedQuantity, customer.Cart.CartItems.Single().Quantity);
    }
    [Fact]
    public void AddCartItem_NullProduct_ReturnsError()
    {
        // Arrange
        Product? product = null;
        var requestedQuantity = 2;

        // Act
        var result = CartOperationService.AddCartItem(customer, product, requestedQuantity);

        // Assert
        Assert.Contains(DomainError.Products.NotFound, result.Errors);
    }

    [Fact]
    public void AddCartItem_NullCustomer_ReturnsError()
    {
        // Arrange
        Customer? customer = null;

        var requestedQuantity = 2;

        // Act
        var result = CartOperationService
            .AddCartItem(customer, product1, requestedQuantity);

        // Assert
        Assert.Contains(DomainError.Customers.NotFound, result.Errors);
    }

    [Fact]
    public void AddCartItem_SameProduct_CombineQuantities()
    {
        // Arrange
        var initialQuantity = 3;
        var additionalQuantity = 2;

        // Act
        var firstAdditionResult = CartOperationService
            .AddCartItem(customer, product1, initialQuantity);

        var secondAdditionResult = CartOperationService
            .AddCartItem(customer, product1, additionalQuantity);

        // Check that the product is in the cart
        var cartItem = customer.Cart.CartItems
            .SingleOrDefault(ci => ci.ProductId == product1.Id);

        Assert.NotNull(cartItem);

        // Check that the quantity is the sum of the initial and additional quantities
        Assert.Equal(initialQuantity + additionalQuantity, cartItem.Quantity);
    }

    [Fact]
    public void AddCartItem_InactiveProduct_ReturnsError()
    {
        // Arrange
        product1.MarkAsDeleted();

        var requestedQuantity = 2;

        // Act
        var result = CartOperationService.AddCartItem(customer, product1, requestedQuantity);

        // Assert
        Assert.Contains(DomainError.Products.Deleted(product1.Name), result.Errors);
    }

    [Fact]
    public void RemoveCartItem_ValidData_CorrectResult()
    {
        // Arrange
        customer.AddCartItem(product1.Id, 3); // Add 3 items to cart
        var requestedQuantity = 2;

        // Act
        var result = CartOperationService.RemoveCartItem(customer, product1, requestedQuantity);

        // Assert
        Assert.Equal(-product1.CustomerPrice * requestedQuantity, result.Value);
        Assert.Equal(1, customer.Cart.CartItems.Single().Quantity);
    }

    [Fact]
    public void RemoveCartItem_NullCustomer_ReturnsError()
    {
        // Arrange
        Customer? customer = null;

        var requestedQuantity = 2;

        // Act
        var result = CartOperationService.RemoveCartItem(customer, product1, requestedQuantity);

        // Assert
        Assert.Contains(DomainError.Customers.NotFound, result.Errors);
    }

    [Fact]
    public void RemoveCartItem_NullProduct_ReturnsError()
    {
        // Arrange
        Product? product = null;
        var requestedQuantity = 2;

        // Act
        var result = CartOperationService.RemoveCartItem(customer, product, requestedQuantity);

        // Assert
        Assert.Contains(DomainError.Products.NotFound, result.Errors);
    }

    [Fact]
    public void RemoveCartItem_RemoveMoreItemsThanInCart_ReturnsError()
    {
        // Arrange
        customer.AddCartItem(product1.Id, 2); // Add 2 items to cart

        // Act
        var result = CartOperationService.RemoveCartItem(customer, product1, 3);

        // Assert
        Assert.Contains(DomainError.CartItems.NegativeQuantity, result.Errors);
    }
}
