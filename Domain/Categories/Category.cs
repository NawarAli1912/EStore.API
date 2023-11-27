using Domain.Products;

namespace Domain.Categories;

public class Category
{
    private readonly HashSet<Product> _products = new();

    private Category(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public IReadOnlyList<Product> Products => _products.ToList();

    public static Category Create(string name)
    {
        return new(Guid.NewGuid(), name);
    }

    private Category()
    {
    }
}
