using Domain.Categories;
using Domain.Kernal;
using MediatR;

namespace Application.Categories.GetFullHierarchy;

public record GetFullHierarchyQuery() : IRequest<Result<List<Category>>>;
