namespace Contracts.Categories;

public record UpdateCategoryRequest(
    string? Name,
    Guid? ParentCategoryId,
    bool NullParent = false);
