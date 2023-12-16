using Domain.Categories;
using MediatR;
using SharedKernel;

namespace Application.Categories.GetFullHierarchy;

public record GetFullHierarchyQuery() : IRequest<Result<List<Category>>>;
