namespace Contracts.Products;

public record AssignUnAssignCategoriesRequest(
    List<Guid> CategoriesIds
    );