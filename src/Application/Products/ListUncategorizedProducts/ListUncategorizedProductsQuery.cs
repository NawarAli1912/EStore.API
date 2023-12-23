using Application.Products.List;
using MediatR;
using SharedKernel.Primitives;

namespace Application.Products.ListUncategorizedProducts;

public sealed record ListUncategorizedProductsQuery(
    int Page,
    int PageSize) : IRequest<Result<ListProductResult>>;
