using Domain.Categories;
using MediatR;
using SharedKernel;

namespace Application.Categories.GetHierarchyDownward;

public record GetHierarchyDownwardQuery(Guid Id) : IRequest<Result<Category>>;
