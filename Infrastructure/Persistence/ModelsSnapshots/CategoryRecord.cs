namespace Infrastructure.Persistence.ModelsSnapshots;

public class CategoryRecord
{
    public Guid CategoryId { get; set; }

    public string CategoryName { get; set; } = default!;

    public Guid? ParentCategoryId { get; set; }
}
