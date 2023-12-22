using MediatR;
using SharedKernel.Primitives;

namespace Application.Categories.Create;

public record CreateCategoryCommand(
    string Name,
    Guid? ParentCategoryId,
    List<Guid> Products) : IRequest<Result<Created>>;
