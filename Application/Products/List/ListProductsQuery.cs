using Domain.Kernal;
using MediatR;

namespace Application.Products.List;

public record ListProductsQuery(
    ProductsFilter Filter,
    string? SortColumn,
    string? SortOrder,
    int Page = 0,
    int PageSize = 10) : IRequest<Result<ListProductResult>>;
