using Domain.Categories;
using MediatR;

namespace Application.Categories.GetHierarchyDownward;

public record GetHierarchyDownwardQuery(Guid Id) : IRequest<Category>;
