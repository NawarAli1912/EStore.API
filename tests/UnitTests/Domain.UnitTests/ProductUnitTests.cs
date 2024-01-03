using Domain.Products.Enums;
using Domain.Products.Errors;
using SharedKernel.Primitives;

namespace Domain.UnitTests;

public class ProductUnitTests
{
    [Fact]
    public void AssignCategories_NewCategories_CategoriesAssigned()
    {
        // Arrange
        var product = TestDataFactory.CreateProduct();
        var category1 = TestDataFactory.CreateCategory("Category1");
        var category2 = TestDataFactory.CreateCategory("Category2");

        // Act
        product.AssignCategories([category1, category2]);

        // Assert
        Assert.Contains(category1, product.Categories);
        Assert.Contains(category2, product.Categories);
    }

    [Fact]
    public void UnassignCategories_NewCategories_CategoriesAssigned()
    {
        // Arrange
        var category1 = TestDataFactory.CreateCategory("Category1");
        var category2 = TestDataFactory.CreateCategory("Category2");
        var product = TestDataFactory.CreateProduct(
            categories: [category1, category2]
            );

        // Act
        product.UnassignCategories([category1, category2]);

        // Assert
        Assert.DoesNotContain(category1, product.Categories);
        Assert.DoesNotContain(category2, product.Categories);
    }

    [Fact]
    public void Update_ValidData_PropertiesUpdated()
    {
        // Arrange
        var product = TestDataFactory.CreateProduct();

        var newName = "ProductNewName";
        var newDescription = "ProductNewDescription";
        var newQuantity = 22;
        var newCustomerPrice = 39.99m;
        var newPurchasePrice = 29.99m;


        // Act
        var result = product.Update(
            newName,
            newDescription,
            newQuantity,
            newCustomerPrice,
            newPurchasePrice).Value;

        // Assert
        Assert.Equal(newName, result.Name);
        Assert.Equal(newDescription, result.Description);
        Assert.Equal(newQuantity, result.Quantity);
        Assert.Equal(newCustomerPrice, result.CustomerPrice);
        Assert.Equal(newPurchasePrice, result.PurchasePrice);
    }

    [Fact]
    public void Update_InvalidQuantity_StockErrorAdded()
    {
        // Arrange
        var product = TestDataFactory.CreateProduct();

        // Act
        var result = product.Update(
            default!,
            default!,
            -1,
            default!,
            default!);

        // Assert
        Assert.Single(result.Errors);
        Assert.Equal(
            DomainError.Product.StockError(product.Name),
            result.Errors.First());
    }

    [Fact]
    public void DecreaseQuantity_WhenQuantityIsGreaterThanZero_ShouldReturnUpdated()
    {
        // Arrange
        var product = TestDataFactory.CreateProduct();
        var quantity = product.Quantity;
        // Act
        var result = product.DecreaseQuantity(2);

        // Assert
        Assert.Equal(Result.Updated, result);
        Assert.Equal(product.Quantity, quantity - 2);
        Assert.Equal(ProductStatus.Active, product.Status);
    }

    [Fact]
    public void DecreaseQuantity_WhenQuantityBecomesZero_ShouldSetStatusToOutOfStock()
    {
        // Arrange
        var product = TestDataFactory.CreateProduct(quantity: 1);

        // Act
        var result = product.DecreaseQuantity(1);

        // Assert
        Assert.Equal(Result.Updated, result);
        Assert.Equal(0, product.Quantity);
        Assert.Equal(ProductStatus.OutOfStock, product.Status);
    }

    [Fact]
    public void DecreaseQuantity_WhenQuantityBecomesNegative_ShouldReturnStockError()
    {
        // Arrange
        var product = TestDataFactory.CreateProduct(quantity: 1);

        // Act
        var result = product.DecreaseQuantity(2);

        // Assert
        Assert.Contains(DomainError.Product.StockError(product.Name), result.Errors);
        Assert.Equal(ProductStatus.Active, product.Status);
    }

    [Fact]
    public void MarkAsDeleted_SetsStatusToDeleted()
    {
        // Arrange
        var product = TestDataFactory.CreateProduct();

        // Act
        product.MarkAsDeleted();

        // Assert
        Assert.Equal(ProductStatus.Deleted, product.Status);
    }

    [Fact]
    public void IncreaseQuantity_ValidAmount_QuantityIncreases()
    {
        // Arrange
        var product = TestDataFactory.CreateProduct(quantity: 10);
        int increaseAmount = 5;

        // Act
        product.IncreaseQuantity(increaseAmount);

        // Assert
        Assert.Equal(15, product.Quantity);
    }
}