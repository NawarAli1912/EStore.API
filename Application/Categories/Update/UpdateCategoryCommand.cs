using Domain.Categories;
using Domain.Kernal;
using MediatR;

namespace Application.Categories.Update;
public record UpdateCategoryCommand(
    Guid Id,
    string? Name,
    Guid? ParentCategoryId,
    bool NullParent = false
    ) : IRequest<Result<Category>>;
