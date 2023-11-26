using Domain.Kernal.ValueObjects;
using Domain.Products.ValueObjects;

namespace Domain.Products;

public class Product
{
    private readonly List<Guid> _cateogries = new();

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public int Quantity { get; private set; }

    public Money Price { get; private set; } = default!;

    public Sku Sku { get; private set; } = default!;

    public IReadOnlyList<Guid> Categories => _cateogries.ToList();
}
