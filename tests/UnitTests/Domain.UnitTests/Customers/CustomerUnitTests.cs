using SharedKernel.Primitives;

namespace Domain.UnitTests.Customers;
public sealed class CustomerUnitTests
{
    [Fact]
    public void AddCartItem_ValidItem_ItemAddedToCart()
    {
        // Arrange
        var customer = TestDataFactory.CreateCustomer();
        var productId = Guid.NewGuid();
        int quantity = 5;

        // Act
        var result = customer.AddCartItem(productId, quantity);

        // Assert
        Assert.Equal(Result.Updated, result);
        var cartItem = customer.Cart.CartItems.FirstOrDefault(item => item.ItemId == productId);
        Assert.NotNull(cartItem);
        Assert.Equal(quantity, cartItem.Quantity);
    }

    [Fact]
    public void RemoveCartItem_ExistingItem_ItemRemovedOrQuantityReduced()
    {
        // Arrange
        var customer = TestDataFactory.CreateCustomer();
        var productId = Guid.NewGuid();
        int addQuantity = 5;
        int removeQuantity = 3;
        customer.AddCartItem(productId, addQuantity);

        // Act
        var result = customer.RemoveCartItem(productId, removeQuantity);

        // Assert
        Assert.Equal(Result.Updated, result);
        var cartItem = customer.Cart.CartItems.FirstOrDefault(item => item.ItemId == productId);
        Assert.NotNull(cartItem);
        Assert.Equal(addQuantity - removeQuantity, cartItem.Quantity);
    }

    [Fact]
    public void RemoveCartItem_ExactQuantity_ItemRemovedFromCart()
    {
        // Arrange
        var customer = TestDataFactory.CreateCustomer();
        var productId = Guid.NewGuid();
        int addQuantity = 5;
        int removeQuantity = 5;
        customer.AddCartItem(productId, addQuantity);

        // Act
        var result = customer.RemoveCartItem(productId, removeQuantity);

        // Assert
        Assert.Equal(Result.Updated, result);
        var cartItem = customer.Cart.CartItems.FirstOrDefault(item => item.ItemId == productId);
        Assert.Null(cartItem);
    }

    [Fact]
    public void ClearCart_WhenCalled_CartIsEmpty()
    {
        // Arrange
        var customer = TestDataFactory.CreateCustomer();
        customer.AddCartItem(Guid.NewGuid(), 5);

        // Act
        customer.ClearCart();

        // Assert
        Assert.Empty(customer.Cart.CartItems);
    }

    [Fact]
    public void AddCartItem_NegativeQuantity_ReturnsError()
    {
        // Arrange
        var customer = TestDataFactory.CreateCustomer();
        var productId = Guid.NewGuid();

        // Act
        var result = customer.AddCartItem(productId, -1);

        // Assert
        Assert.True(result.IsError);
        // Assert specific error message or type
    }

    [Fact]
    public void RemoveCartItem_NonExistingItem_ReturnsError()
    {
        // Arrange
        var customer = TestDataFactory.CreateCustomer();
        var productId = Guid.NewGuid();

        // Act
        var result = customer.RemoveCartItem(productId, 1);

        // Assert
        Assert.True(result.IsError);
        // Assert specific error message or type
    }


}
