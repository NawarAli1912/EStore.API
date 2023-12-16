using MediatR;
using SharedKernel;

namespace Application.Categories.Create;

public record CreateCategoryCommand(
    string Name,
    Guid? ParentCategoryId,
    List<Guid> Products) : IRequest<Result<Created>>;
