using Application.Products.Filters;
using Domain.Kernal;
using MediatR;

namespace Application.Products.List;

public record ListProductsQuery(
    ListProductFilter Filter,
    string? SortColumn,
    string? SortOrder,
    int Page = 0,
    int PageSize = 10) : IRequest<Result<ListProductResult>>;
