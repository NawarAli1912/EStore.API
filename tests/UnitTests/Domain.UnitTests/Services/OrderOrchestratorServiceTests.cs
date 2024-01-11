using Domain.Errors;
using Domain.Offers;
using Domain.Offers.Enums;
using Domain.Products;
using Domain.Services;
using SharedKernel.Enums;

namespace Domain.UnitTests.Services;
public sealed class OrderOrchestratorServiceTests
{
    private readonly Product _p1 = TestDataFactory.CreateProduct(customerPrice: 99.0m);
    private readonly Product _p2 = TestDataFactory.CreateProduct(customerPrice: 19.99m);
    private readonly Product _p3 = TestDataFactory.CreateProduct(customerPrice: 4.99m);

    private readonly ShippingCompany _shippingCompany = ShippingCompany.Alkadmous;
    private readonly string _shippingCompanyAddress = "Downtown";
    private readonly string _phoneNumber = "+963992465535";
    private readonly List<Product> _products;
    private readonly List<Offer> _offers;
    private readonly Offer _percentageOffer;
    private readonly Offer _bundleOffer;

    public OrderOrchestratorServiceTests()
    {
        _products = [_p1, _p2, _p3];

        _percentageOffer = TestDataFactory.CreateOffer(
            OfferType.PercentageDiscountOffer,
            [_p1.Id]);

        _bundleOffer = TestDataFactory.CreateOffer(
            OfferType.BundleDiscountOffer,
            _products.Select(p => p.Id).ToList());

        _offers = [_percentageOffer, _bundleOffer];
    }

    [Fact]
    public void CreateOrderWithEmptyCart_ReturnsError()
    {
        // Arrange
        var customer = TestDataFactory.CreateCustomer();

        var ordersService = new OrderOrchestratorService(_products, _offers);

        // Act 
        var result = ordersService.CreateOrder(
            customer,
            _shippingCompany,
            _shippingCompanyAddress,
            _phoneNumber);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(DomainError.Carts.EmptyCart, result.Errors);
    }

    [Fact]
    public void CreateOrderWithUnpresentProductId_ReturnsError()
    {
        // Arrange
        var dummyProduct = TestDataFactory.CreateProduct();

        var customer = TestDataFactory.CreateCustomer();
        customer.AddCartItem(dummyProduct.Id, 2);

        var ordersService = new OrderOrchestratorService(
                _products,
                _offers);

        // Act 
        var result = ordersService.CreateOrder(
            customer,
            _shippingCompany,
            _shippingCompanyAddress,
            _phoneNumber);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(DomainError.Products.NotPresentOnTheDictionary, result.Errors);
    }

    [Fact]
    public void CreateOrderWithDeletedOrOutOfStockProducts_ReturnError()
    {

        var customer = TestDataFactory.CreateCustomer();
        customer.AddCartItem(_p1.Id, 3);
        customer.AddCartItem(_p2.Id, 2);

        _p1.MarkAsDeleted();
        _p2.DecreaseQuantity(_p2.Quantity);

        var ordersService = new OrderOrchestratorService(
                _products,
                _offers);

        // Act 
        var result = ordersService.CreateOrder(
            customer,
            _shippingCompany,
            _shippingCompanyAddress,
            _phoneNumber);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(
            DomainError.Products.Deleted(_p1.Name),
            result.Errors);
        Assert.Contains(
            DomainError.Products.OutOfStock(_p2.Name),
            result.Errors);
    }

    [Fact]
    public void CreateOrderWithUnpresentOfferId_ReturnsError()
    {
        var dummyOffer = TestDataFactory.CreateOffer(
            OfferType.PercentageDiscountOffer,
            [_p1.Id]);

        var customer = TestDataFactory.CreateCustomer();
        customer.AddCartItem(dummyOffer.Id, 2, ItemType.Offer);

        var ordersService = new OrderOrchestratorService(
                _products,
                _offers);

        // Act 
        var result = ordersService.CreateOrder(
            customer,
            _shippingCompany,
            _shippingCompanyAddress,
            _phoneNumber);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(DomainError.Offers.NotPresentOnTheDictionary, result.Errors);
    }

    [Theory]
    [InlineData(1, 1, 1, 1, 1)]
    [InlineData(1, 2, 3, 4, 5)]
    [InlineData(3, 2, 5, 2, 1)]
    [InlineData(20, 22, 23, 24, 39)]
    [InlineData(50, 22, 18, 24, 39)]
    public void CreateOrderWithValidData_CalculateOrderTotalPriceRight(
        int p1Quantity,
        int p2Quantity,
        int p3Quantity,
        int percentageOfferQuantity,
        int bundleOfferQuantity)
    {
        var customer = TestDataFactory.CreateCustomer();
        customer.AddCartItem(_p1.Id, p1Quantity);
        customer.AddCartItem(_p2.Id, p2Quantity);
        customer.AddCartItem(_p3.Id, p3Quantity);
        customer.AddCartItem(_percentageOffer.Id, percentageOfferQuantity, ItemType.Offer);
        customer.AddCartItem(_bundleOffer.Id, bundleOfferQuantity, ItemType.Offer);

        var ExpectedTotalPrice = 0.0m;
        ExpectedTotalPrice += _p1.CustomerPrice * p1Quantity;
        ExpectedTotalPrice += _p2.CustomerPrice * p2Quantity;
        ExpectedTotalPrice += _p3.CustomerPrice * p3Quantity;
        ExpectedTotalPrice += _bundleOffer.CalculatePrice(
            _products
            .ToDictionary(p => p.Id, p => p.CustomerPrice)) * bundleOfferQuantity;
        ExpectedTotalPrice += _percentageOffer.CalculatePrice(
            _products.ToDictionary(p => p.Id, p => p.CustomerPrice)) * percentageOfferQuantity;


        var ordersService = new OrderOrchestratorService(
                _products,
                _offers);

        // Act 
        var result = ordersService.CreateOrder(
            customer,
            _shippingCompany,
            _shippingCompanyAddress,
            _phoneNumber);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(ExpectedTotalPrice, result.Value.TotalPrice);
    }

    [Theory]
    [InlineData(1, 1, 1, 1, 1)]
    [InlineData(1, 2, 3, 4, 5)]
    [InlineData(3, 2, 5, 2, 1)]
    [InlineData(20, 22, 23, 24, 39)]
    [InlineData(50, 22, 18, 24, 39)]
    public void CreateOrderWithValidData_OrderLineItemsShouldMatchTheCart(
        int p1Quantity,
        int p2Quantity,
        int p3Quantity,
        int percentageOfferQuantity,
        int bundleOfferQuantity)
    {
        var customer = TestDataFactory.CreateCustomer();
        customer.AddCartItem(_p1.Id, p1Quantity);
        customer.AddCartItem(_p2.Id, p2Quantity);
        customer.AddCartItem(_p3.Id, p3Quantity);
        customer.AddCartItem(_percentageOffer.Id, percentageOfferQuantity, ItemType.Offer);
        customer.AddCartItem(_bundleOffer.Id, bundleOfferQuantity, ItemType.Offer);

        var ordersService = new OrderOrchestratorService(
                _products,
                _offers);

        // Act 
        var result = ordersService.CreateOrder(
            customer,
            _shippingCompany,
            _shippingCompanyAddress,
            _phoneNumber);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(p1Quantity,
            result.Value.LineItems.Where(li => li.ProductId == _p1.Id &&
            li.Type == ItemType.Product).Count());
        Assert.Equal(p2Quantity,
            result.Value.LineItems.Where(li => li.ProductId == _p2.Id &&
            li.Type == ItemType.Product).Count());
        Assert.Equal(p3Quantity,
            result.Value.LineItems.Where(li => li.ProductId == _p3.Id &&
            li.Type == ItemType.Product).Count());
        Assert.Equal(percentageOfferQuantity,
            result.Value.LineItems.Where(li => li.ProductId == _percentageOffer.ListRelatedProductsIds().First() &&
            li.RelatedOfferId == _percentageOffer.Id).Count());

        foreach (var productId in _bundleOffer.ListRelatedProductsIds())
        {
            Assert.Equal(bundleOfferQuantity, result.Value.LineItems.Where(li => li.ProductId == productId &&
                            li.RelatedOfferId == _bundleOffer.Id).Count());
        }
    }
}
