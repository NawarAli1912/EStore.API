using MediatR;
using SharedKernel.Primitives;

namespace Application.Products.UnassignCategories;
public record UnassignCategoriesCommand(
    Guid Id,
    List<Guid> CategoriesIds
    ) : IRequest<Result<Updated>>;