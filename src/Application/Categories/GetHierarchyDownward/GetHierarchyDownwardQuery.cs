using Domain.Categories;
using MediatR;
using SharedKernel.Primitives;

namespace Application.Categories.GetHierarchyDownward;

public record GetHierarchyDownwardQuery(Guid Id) : IRequest<Result<Category>>;
