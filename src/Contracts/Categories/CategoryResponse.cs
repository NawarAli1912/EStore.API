namespace Contracts.Categories;
public record CategoryResponse(
    Guid Id,
    string Name,
    List<CategoryResponse> SubCategories);
