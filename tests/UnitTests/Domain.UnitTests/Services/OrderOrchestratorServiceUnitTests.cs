using Domain.Customers;
using Domain.Errors;
using Domain.Orders.Enums;
using Domain.Products;
using Domain.Products.Enums;
using Domain.Services;
using SharedKernel.Enums;
using SharedKernel.Primitives;

namespace Domain.UnitTests.Services;
public sealed class OrderOrchestratorServiceUnitTests
{
    private readonly Customer customer;
    private readonly Dictionary<Guid, Product> productDict;
    private readonly Product product1;
    private readonly Product product2;

    private readonly ShippingCompany shippingCompany = ShippingCompany.Alkadmous;
    private readonly string shippingLocation = "Location1";
    private readonly string phone = "963992465535";


    public OrderOrchestratorServiceUnitTests()
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

    [Theory]
    [InlineData(2.0)]
    [InlineData(100.0)]
    [InlineData(3000.0)]
    public void CreateOrder_ValidData_OrderCreatedSuccessfully(decimal customerPrice)
    {
        // Arrange
        var product1 = TestDataFactory.CreateProduct();
        var product2 = TestDataFactory.CreateProduct(purchasePrice: customerPrice - 1, customerPrice: customerPrice);
        Dictionary<Guid, Product> productDict = [];
        productDict.Add(product1.Id, product1);
        productDict.Add(product2.Id, product2);

        customer.AddCartItem(product1.Id, 2);
        customer.AddCartItem(product2.Id, 5);

        var orderExpectedTotalPrice =
            2 * product1.CustomerPrice + 5 * product2.CustomerPrice;


        // Act
        var result = OrderOrchestratorService.CreateOrder(
            customer,
            productDict,
            shippingCompany,
            shippingLocation,
            phone);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Empty(customer.Cart.CartItems);
        Assert.Contains(product1.Id, result.Value.LineItems.Select(li => li.ProductId));
        Assert.Equal(5, result.Value.LineItems.Where(li => li.ProductId == product2.Id).Count());
        Assert.Equal(orderExpectedTotalPrice, result.Value.TotalPrice);
        Assert.Equal(0, customer.Cart.CartItems.Count);
    }

    [Fact]
    public void CreateOrder_DeletedProductOrOutOfStock_ReturnsError()
    {
        // Arrange
        customer.AddCartItem(product1.Id, 1);
        customer.AddCartItem(product2.Id, product2.Quantity);
        product1.MarkAsDeleted();
        product2.DecreaseQuantity(product2.Quantity);

        // Act
        var result = OrderOrchestratorService.CreateOrder(
            customer,
            productDict,
            shippingCompany,
            shippingLocation,
            phone);
        // Assert
        Assert.Contains(DomainError.Products.Deleted(product1.Name), result.Errors);
        Assert.Contains(DomainError.Products.OutOfStock(product2.Name), result.Errors);
    }

    [Fact]
    public void CreateOrder_EmptyCart_ReturnsError()
    {
        customer.ClearCart();

        // Act
        var result = OrderOrchestratorService.CreateOrder(
            customer,
            productDict,
            shippingCompany,
            shippingLocation,
            phone);

        // Assert
        Assert.Contains(DomainError.Carts.EmptyCart, result.Errors);
    }

    [Fact]
    public void CreateOrder_InvalidProduct_ReturnsError()
    {
        product1.MarkAsDeleted();
        product2.DecreaseQuantity(product2.Quantity);

        customer.AddCartItem(Guid.NewGuid(), 1);

        // Act
        var result = OrderOrchestratorService.CreateOrder(
            customer,
            productDict,
            shippingCompany,
            shippingLocation,
            phone);

        // Assert
        Assert.Contains(DomainError.Products.NotFound, result.Errors);
    }

    [Fact]
    public void Approve_OrderWithInactiveProduct_ReturnsError()
    {
        customer.AddCartItem(product1.Id, 2);

        var order = OrderOrchestratorService.CreateOrder(
            customer,
            productDict,
            shippingCompany,
            shippingLocation,
            phone).Value;

        product1.MarkAsDeleted();
        customer.ClearCart();

        // Act
        var result = OrderOrchestratorService.Approve(order, productDict);

        // Assert
        Assert.True(result.IsError);
    }

    [Fact]
    public void UpdateItems_ValidData_UpdatesOrderSuccessfully()
    {
        // Arrange
        customer.AddCartItem(product1.Id, 1);
        customer.AddCartItem(product2.Id, 2);

        var order = OrderOrchestratorService.CreateOrder(
            customer,
            productDict,
            shippingCompany,
            shippingLocation,
            phone).Value;

        var itemsToAddQuantities = new Dictionary<Guid, int> { { product1.Id, 2 } };
        var itemsToDeleteQuantities = new Dictionary<Guid, int> { { product2.Id, 1 } };

        // Act
        var result = OrderOrchestratorService.UpdateItems(
            order,
            productDict,
            itemsToAddQuantities,
            itemsToDeleteQuantities);

        // Assert
        Assert.Equal(Result.Updated, result);
        Assert.Equal(4, order.LineItems.Count);
        Assert.Equal(3, order.LineItems.Where(li => li.ProductId == product1.Id).Count());
    }

    [Fact]
    public void UpdateItems_WithNonExistentProduct_ReturnsError()
    {
        // Arrange        
        customer.AddCartItem(product1.Id, 2);
        customer.AddCartItem(product2.Id, 12);

        var order = OrderOrchestratorService.CreateOrder(
            customer,
            productDict,
            shippingCompany,
            shippingLocation,
            phone).Value;

        var itemsToAddQuantities = new Dictionary<Guid, int> { { Guid.NewGuid(), 1 } };
        var itemsToDeleteQuantities = new Dictionary<Guid, int>();

        // Act
        var result = OrderOrchestratorService.UpdateItems(order, productDict, itemsToAddQuantities, itemsToDeleteQuantities);

        // Assert
        Assert.True(result.IsError);
    }

    [Fact]
    public void Approve_SuccessfulOrderApproval_DecreasesProductQuantities()
    {
        // Arrange
        foreach (var product in productDict.Values)
        {
            customer.AddCartItem(product.Id, 1);
        }

        var order = OrderOrchestratorService.CreateOrder(
            customer,
            productDict,
            shippingCompany,
            shippingLocation,
            phone).Value;

        var initialQuantities = productDict.Values.ToDictionary(product => product.Id, product => product.Quantity);

        // Act
        var result = OrderOrchestratorService.Approve(order, productDict);

        // Assert
        Assert.Equal(Result.Updated, result);
        foreach (var lineItem in order.LineItems)
        {
            var product = productDict[lineItem.ProductId];
            Assert.Equal(initialQuantities[product.Id] - 1, product.Quantity);
        }
    }

    [Fact]
    public void Approve_OrderApproval_ProductStatusUpdated()
    {
        // Arrange
        customer.AddCartItem(product1.Id, product1.Quantity);

        var order = OrderOrchestratorService.CreateOrder(
            customer,
            productDict,
            shippingCompany,
            shippingLocation,
            phone).Value;

        // Act
        var result = OrderOrchestratorService.Approve(order, productDict);

        // Assert
        Assert.Equal(Result.Updated, result);
        Assert.Equal(ProductStatus.OutOfStock, product1.Status);
    }

    [Fact]
    public void Approve_SuccessfulOrderApproval_UpdatesOrderStatus()
    {
        // Arrange
        var product = productDict.Values.First();
        customer.AddCartItem(product.Id, 2);


        var order = OrderOrchestratorService.CreateOrder(
            customer,
            productDict,
            shippingCompany,
            shippingLocation,
            phone).Value;

        // Act
        var result = OrderOrchestratorService.Approve(order, productDict);

        // Assert
        Assert.Equal(Result.Updated, result);
        Assert.Equal(OrderStatus.Approved, order.Status);
    }

    [Fact]
    public void Approve_WithEmptyProductDictionary_ShouldReturnError()
    {
        // Arrange
        customer.AddCartItem(product1.Id, 2);
        var order = OrderOrchestratorService.CreateOrder(
            customer,
            productDict,
            shippingCompany,
            shippingLocation,
            phone).Value;

        // Act
        var result = OrderOrchestratorService.Approve(order, []);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(DomainError.Products.NotPresentOnTheDictionary, result.Errors);
    }

}
