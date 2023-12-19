using MediatR;
using SharedKernel;

namespace Application.Products.AssignCategories;

public record AssignCategoriesCommand(
    Guid Id,
    List<Guid> CategoriesIds
    ) : IRequest<Result<Updated>>;
