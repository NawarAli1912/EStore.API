using Domain.Categories;
using MediatR;
using SharedKernel.Primitives;

namespace Application.Categories.Update;
public record UpdateCategoryCommand(
    Guid Id,
    string? Name,
    Guid? ParentCategoryId,
    bool NullParent = false
    ) : IRequest<Result<Category>>;
