using Domain.Categories;
using Domain.Kernal.Models;
using Domain.Kernal.ValueObjects;
using Domain.Products.ValueObjects;

namespace Domain.Products;

public class Product : AggregateRoot<Guid>
{
    private readonly List<Category> _cateogries = new();

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public int Quantity { get; private set; }

    public Money Price { get; private set; } = default!;

    public Sku Sku { get; private set; } = default!;

    public IReadOnlyList<Category> Categories => _cateogries.ToList();

    private Product() : base(Guid.NewGuid())
    {
    }
}
