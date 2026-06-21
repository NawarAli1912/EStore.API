using Domain.Categories;
using SharedKernel.Messaging;
using SharedKernel.Primitives;

namespace Application.Categories.GetHierarchyDownward;

public record GetHierarchyDownwardQuery(Guid Id) : IRequest<Result<Category>>;
