using MediatR;
using SharedKernel;

namespace Application.Products.List;

public record ListProductsQuery(
    ProductsFilter Filter,
    string? SortColumn,
    string? SortOrder,
    int Page = 1,
    int PageSize = 10) : IRequest<Result<ListProductResult>>;
