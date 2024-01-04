using Domain.Categories;
using Domain.Customers;
using Domain.Orders;
using Domain.Products;
using SharedKernel.Enums;

namespace Domain.UnitTests;
internal static class TestDataFactory
{
    public static Customer CreateCustomer() =>
        Customer.Create(Guid.NewGuid());

    public static Product CreateProduct(
        string name = "TestProduct",
        string description = "TestDescription",
        int quantity = 100,
        decimal customerPrice = 99M,
        decimal purchasePrice = 119M,
        List<Category>? categories = null) =>
        Product.Create(
        Guid.NewGuid(),
        name,
        description,
        quantity,
        customerPrice,
        purchasePrice,
        categories);

    private static List<Category> GeneratesubCategories(Guid parentId)
    {
        return Enumerable.Range(1, 3)
        .Select(i => Category.Create(Guid.NewGuid(), $"SubCategory{i}", parentId, null))
        .ToList();
    }

    public static Category CreateCategory(string name = "TestCategory")
    {
        Guid id = Guid.NewGuid();

        return Category.Create(id, name, null, null, GeneratesubCategories(id));
    }

    public static Order CreateOrder(Customer? customer = null)
    {
        customer ??= CreateCustomer();

        return Order.Create(
            customer.Id,
            ShippingCompany.Alkadmous,
            "Location1",
            "+963992465535");
    }
}
