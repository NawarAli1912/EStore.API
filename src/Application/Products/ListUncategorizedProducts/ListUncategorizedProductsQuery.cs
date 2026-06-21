using Application.Products.List;
using SharedKernel.Messaging;
using SharedKernel.Primitives;

namespace Application.Products.ListUncategorizedProducts;

public sealed record ListUncategorizedProductsQuery(
    int Page,
    int PageSize) : IRequest<Result<ListProductResult>>;
