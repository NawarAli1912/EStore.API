namespace Contracts.Categories;

public record CreateCategoryRequest(
    string Name,
    Guid? ParentCategoryId,
    List<Guid>? Products
    );
