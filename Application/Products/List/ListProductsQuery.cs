using Application.Common.Models;
using Domain.Kernal;
using Domain.Products;
using MediatR;

namespace Application.Products.List;

public record ListProductsQuery(
    string? SearchTerm,
    string? SortColumn,
    string? SortOrder,
    int Page = 0,
    int PageSize = 10) : IRequest<Result<PagedList<Product>>>;
