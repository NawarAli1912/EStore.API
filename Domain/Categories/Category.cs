using Domain.Kernal.Models;
using Domain.Products;

namespace Domain.Categories;

public sealed class Category : AggregateRoot<Guid>
{
    private readonly HashSet<Product> _products = [];
    private readonly List<Category> _subCategories = [];

    public string Name { get; private set; } = string.Empty;

    public Guid? ParentCategoryId { get; private set; }

    public Category ParentCategory { get; private set; }

    public IReadOnlySet<Product> Products => _products.ToHashSet();
    public IReadOnlyList<Category> SubCategories => _subCategories.ToList();

    public static Category Create(string name, Guid? parentCategoryId = null)
    {
        return new Category
        {
            Name = name,
            ParentCategoryId = parentCategoryId
        };
    }

    private Category() : base(Guid.NewGuid())
    {
    }
}
