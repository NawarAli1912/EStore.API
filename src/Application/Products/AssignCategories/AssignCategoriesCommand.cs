using MediatR;
using SharedKernel.Primitives;

namespace Application.Products.AssignCategories;

public record AssignCategoriesCommand(
    Guid Id,
    List<Guid> CategoriesIds
    ) : IRequest<Result<Updated>>;
