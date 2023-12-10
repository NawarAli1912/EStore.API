namespace Contracts.Products;

public record AssignCategoriesRequest(
    List<Guid> CategoriesIds
    );