using Domain.Categories;
using MediatR;

namespace Application.Categories.GetFullHierarchy;

public record GetFullHierarchyQuery() : IRequest<List<Category>>;
