using Domain.Kernal;
using MediatR;

namespace Application.Products.AssignCategories;

public record AssignCategoriesCommand(
    Guid Id,
    List<Guid> CategoriesIds
    ) : IRequest<Result<bool>>;
