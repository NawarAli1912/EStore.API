using Domain.Categories;
using SharedKernel.Messaging;
using SharedKernel.Primitives;

namespace Application.Categories.GetFullHierarchy;

public record GetFullHierarchyQuery() : IRequest<Result<List<Category>>>;
