namespace Domain.Categories;

public class Category
{
    private readonly HashSet<Guid> _products = new();

    private Category(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public IReadOnlyList<Guid> Products => _products.ToList();

    public static Category Create(string name)
    {
        return new(Guid.NewGuid(), name);
    }
}
