using Domain.Categories;
using Domain.Kernal;
using Domain.Kernal.Models;
using Domain.Kernal.ValueObjects;
using Domain.Products.ValueObjects;

namespace Domain.Products;

public class Product : AggregateRoot<Guid>
{
    private readonly List<Category> _cateogries = [];

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public int Quantity { get; private set; }

    public Money Price { get; private set; } = default!;

    public Sku? Sku { get; private set; } = default;

    public IReadOnlyList<Category> Categories => _cateogries.ToList();

    private Product(Guid id) : base(id)
    {
    }

    public static Result<Product> Create(
        string name,
        string description,
        int quantity,
        decimal price,
        string currency,
        string? sku)
    {
        List<Error> errors = [];
        var moneyResult = Money.Create(price, currency);
        if (moneyResult.IsError)
        {
            errors.AddRange(moneyResult.Errors);
        }

        var skuResult = Sku.Create(sku);
        if (skuResult.IsError)
        {
            errors.AddRange(skuResult.Errors);
        }

        if (errors.Count > 0)
        {
            return errors;
        }

        return new Product(Guid.NewGuid())
        {
            Name = name,
            Description = description,
            Quantity = quantity,
            Price = moneyResult.Value,
            Sku = skuResult.Value
        };

    }
}
