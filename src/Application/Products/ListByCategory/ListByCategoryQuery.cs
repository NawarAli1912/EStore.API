using Application.Products.List;
using SharedKernel.Messaging;
using SharedKernel.Primitives;

namespace Application.Products.ListByCategory;

public record ListByCategoryQuery(
    Guid CategoryId,
    int Page = 1,
    int PageSize = 10) : IRequest<Result<ListProductResult>>;
