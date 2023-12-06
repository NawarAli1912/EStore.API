namespace Domain.ModelsSnapshots;

public sealed class CategorySnapshot
{
    public Guid CategoryId { get; set; }

    public string CategoryName { get; set; } = default!;

    public Guid? ParentCategoryId { get; set; }
}
