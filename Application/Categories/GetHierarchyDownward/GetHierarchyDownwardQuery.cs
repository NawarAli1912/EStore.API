using Domain.Categories;
using Domain.Kernal;
using MediatR;

namespace Application.Categories.GetHierarchyDownward;

public record GetHierarchyDownwardQuery(Guid Id) : IRequest<Result<Category>>;
