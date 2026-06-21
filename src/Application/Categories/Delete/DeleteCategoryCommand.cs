using Domain.Categories.Enums;
using SharedKernel.Messaging;
using SharedKernel.Primitives;

namespace Application.Categories.Delete;
public record DeleteCategoryCommand(Guid Id, SubcategoryActions Action) : IRequest<Result<Deleted>>;
