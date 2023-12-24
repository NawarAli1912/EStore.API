using Domain.Categories;
using Domain.Products;
using Domain.Products.Enums;
using Domain.Products.Errors;
using SharedKernel.Primitives;

namespace Domain.UnitTests;

public class ProductUnitTests
{
    private static string CategoryName(int NameModifier) => $"CategoryName{NameModifier}";
    private static string ProductName(int NameModifier) => "ProductName" + NameModifier.ToString();
    private static string ProductDescription(int NameModifier) => "Description" + NameModifier.ToString();
    private static readonly int HighQuantity = 10;
    private static readonly int LowQuantity = 1;
    private static readonly decimal CustomerPrice = 20M;
    private static readonly decimal PurchasePrice = 16M;



    [Fact]
    public void AssignCategories_NewCategories_CategoriesAssigned()
    {
        // Arrange
        var product = Product.Create(
            Guid.NewGuid(),
            ProductName(1),
            ProductDescription(1),
            HighQuantity,
            CustomerPrice,
            PurchasePrice);

        var category1 = Category.Create(Guid.NewGuid(), CategoryName(1));

        var category2 = Category.Create(Guid.NewGuid(), CategoryName(2));

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
        var category1 = Category.Create(Guid.NewGuid(), CategoryName(1));
        var category2 = Category.Create(Guid.NewGuid(), CategoryName(2));
        var product = Product.Create(
            Guid.NewGuid(),
            ProductName(1),
            ProductDescription(1),
            HighQuantity,
            CustomerPrice,
            PurchasePrice,
            [category1, category2]);


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
        var product = Product.Create(
            Guid.NewGuid(),
            ProductName(1),
            ProductDescription(1),
            HighQuantity,
            CustomerPrice,
            PurchasePrice);

        var newName = ProductName(2);
        var newDescription = ProductDescription(2);
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
        var product = Product.Create(
            Guid.NewGuid(),
            ProductName(1),
            ProductDescription(1),
            HighQuantity,
            CustomerPrice,
            PurchasePrice);

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
        var product = Product.Create(
            Guid.NewGuid(),
            ProductName(1),
            ProductDescription(1),
            HighQuantity,
            CustomerPrice,
            PurchasePrice);

        // Act
        var result = product.DecreaseQuantity(2);

        // Assert
        Assert.Equal(Result.Updated, result);
        Assert.Equal(product.Quantity, HighQuantity - 2);
        Assert.Equal(ProductStatus.Active, product.Status);
    }

    [Fact]
    public void DecreaseQuantity_WhenQuantityBecomesZero_ShouldSetStatusToOutOfStock()
    {
        // Arrange
        var product = Product.Create(
            Guid.NewGuid(),
            ProductName(1),
            ProductDescription(1),
            1,
            CustomerPrice,
            PurchasePrice);

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
        var product = Product.Create(
            Guid.NewGuid(),
            ProductName(1),
            ProductDescription(1),
            1,
            CustomerPrice,
            PurchasePrice);

        // Act
        var result = product.DecreaseQuantity(2);

        // Assert
        Assert.Contains(DomainError.Product.StockError(product.Name), result.Errors);
        Assert.Equal(ProductStatus.Active, product.Status); // Ensure status remains unchanged
    }



}