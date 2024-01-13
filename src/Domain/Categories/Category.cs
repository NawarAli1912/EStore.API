using Domain.Categories.Enums;
using Domain.Products;
using SharedKernel.Primitives;

namespace Domain.Categories;

public sealed class Category : AggregateRoot
{
    private readonly HashSet<Product> _products = [];
    private readonly List<Category> _subCategories;

    public string Name { get; private set; } = string.Empty;

    public Guid? ParentCategoryId { get; private set; }

    public IReadOnlySet<Product> Products => _products;

    public IReadOnlyList<Category> SubCategories => _subCategories;

    public static Category Create(
        Guid id,
        string name,
        Guid? parentCategoryId = null,
        Category? parentCategory = null,
        List<Category>? subCategories = null)
    {
        return new Category(
            id,
            name,
            parentCategory,
            subCategories ?? [],
            parentCategoryId
        );
    }

    public static List<Category> BuildCategoryTree(
        List<Category> categories,
        Guid? parentId = null)
    {
        var tree = categories
            .Where(c => c.ParentCategoryId == parentId)
            .Select(c => new Category(
                c.Id,
                c.Name,
                null!,
                BuildCategoryTree(categories, c.Id),
                parentId))
            .ToList();

        return tree;
    }

    public void AssignProducts(IEnumerable<Product> products)
    {
        foreach (var product in products)
        {
            _products.Add(product);
        }
    }

    public void Update(string? name, Guid? parentId, bool nullParent)
    {
        Name = name ?? Name;

        ParentCategoryId = parentId;
        if (nullParent)
        {
            ParentCategoryId = null;
        }
    }

    public void PrepareDelete(SubcategoryActions action)
    {
        var parentId = ParentCategoryId;

        if (action == SubcategoryActions.Detach)
        {
            parentId = null;
        }

        foreach (var subCategory in _subCategories)
        {
            subCategory.ParentCategoryId = parentId;
        }
    }

    private Category(
        Guid id,
        string name,
        Category? parentCategory,
        List<Category> subCategories,
        Guid? parentCategoryId = null) : base(id)
    {
        Name = name;
        ParentCategoryId = parentCategoryId;
        _subCategories = subCategories;
    }

    private Category() : base(Guid.NewGuid())
    {
        _subCategories = [];
    }
}
