using Domain.Kernal.Models;
using Domain.Products;

namespace Domain.Categories;

public sealed class Category : AggregateRoot<Guid>
{
    private readonly HashSet<Product> _products = [];

    public string Name { get; private set; } = string.Empty;

    public IReadOnlyList<Product> Products => _products.ToList();

    public static Category Create(string name)
    {
        return new Category
        {
            Name = name
        };
    }

    private Category() : base(Guid.NewGuid())
    {
    }
}
