﻿using Domain.Customers;
using Domain.Errors;
using Domain.Orders.ValueObjects;
using Domain.Products;

namespace Domain.UnitTests.Orders;
public sealed class OrderUnitTests
{
    private readonly Customer customer;
    private readonly ShippingInfo shippingInfo;

    private readonly Product product1;

    private readonly Product product2;

    public OrderUnitTests()
    {
        customer = TestDataFactory.CreateCustomer();
        product1 = TestDataFactory.CreateProduct(
                    "Product1",
                    "Description1",
                    20,
                    99,
                    80);
        product2 = TestDataFactory.CreateProduct(
                   "Product2",
                   "Description2",
                   12,
                   199,
                   150);
    }

    [Fact]
    public void AddItems_ValidProductAndQuantity_ItemsAddedToOrder()
    {
        // Arrange
        var order = TestDataFactory.CreateOrder();

        var initialItemCount = order.LineItems.Count;
        var initialTotalPrice = order.TotalPrice;

        // Act
        order.AddItems(product1.Id, product1.CustomerPrice, 3);
        order.AddItems(product2.Id, product2.CustomerPrice, 2);

        // Assert
        Assert.Equal(initialItemCount + 5, order.LineItems.Count);
        Assert.Equal(
            2 * product2.CustomerPrice + 3 * product1.CustomerPrice + initialTotalPrice,
            order.TotalPrice);

        Assert.Contains(product1.Id, order.LineItems.Select(li => li.ProductId));
        Assert.Equal(3, order.LineItems.Count(li => li.ProductId == product1.Id));
        Assert.Equal(2, order.LineItems.Count(li => li.ProductId == product2.Id));
    }

    [Fact]
    public void RemoveItems_QuantityExceedsAvailable_ReturnsErrorResult()
    {
        // Arrange
        var order = TestDataFactory.CreateOrder();

        order.AddItems(product1.Id, product1.CustomerPrice, 1);

        var initialItemCount = order.LineItems.Count;

        // Act
        var result = order.RemoveItems(product1.Id, 2);

        // Assert
        Assert.Contains(DomainError.LineItem.ExceedsAvailableQuantity(product1.Id), result.Errors);
        Assert.Equal(1, initialItemCount);
    }

    [Fact]
    public void RemoveItems_ValidProductAndQuantity_ItemsRemovedFromOrder()
    {
        // Arrange
        var order = TestDataFactory.CreateOrder();

        order.AddItems(product1.Id, product1.CustomerPrice, 1);
        order.AddItems(product2.Id, product2.CustomerPrice, 3);

        var initialItemCount = order.LineItems.Count;
        var initialToatalPrice = order.TotalPrice;

        // Act
        order.RemoveItems(product1.Id, 1);
        order.RemoveItems(product2.Id, 2);

        // Assert
        Assert.Equal(initialItemCount - 3, order.LineItems.Count);
        Assert.Equal(
            initialToatalPrice - product1.CustomerPrice - product2.CustomerPrice * 2,
            order.TotalPrice);
        Assert.DoesNotContain(product1.Id, order.LineItems.Select(li => li.ProductId));

    }
}
