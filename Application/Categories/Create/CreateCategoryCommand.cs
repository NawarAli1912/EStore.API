using Domain.Kernal;
using MediatR;

namespace Application.Categories.Create;

public record CreateCategoryCommand(
    string Name,
    Guid? ParentCategoryId,
    List<Guid> Products) : IRequest<Result<Created>>;
