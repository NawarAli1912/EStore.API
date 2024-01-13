using Domain.Categories;

namespace Domain.ModelsSnapshots;

public sealed class CategorySnapshot
{
    public Guid CategoryId { get; set; }

    public string CategoryName { get; set; } = default!;

    public Guid? ParentCategoryId { get; set; }

    public static CategorySnapshot Snapshot(Category category)
    {
        return new CategorySnapshot
        {
            CategoryId = category.Id,
            CategoryName = category.Name
        };
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        CategorySnapshot other = (CategorySnapshot)obj;

        // Compare relevant properties for equality
        return CategoryId == other.CategoryId
            && CategoryName == other.CategoryName
            && ParentCategoryId == other.ParentCategoryId;
    }

    public override int GetHashCode()
    {
        // Implement GetHashCode based on the properties used in Equals
        return HashCode.Combine(CategoryId, CategoryName, ParentCategoryId);
    }

}
