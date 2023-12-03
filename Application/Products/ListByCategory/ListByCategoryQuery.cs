using Application.Products.List;
using Domain.Kernal;
using MediatR;

namespace Application.Products.ListByCategory;

public record ListByCategoryQuery(
    Guid CategoryId,
    int Page = 0,
    int PageSize = 10) : IRequest<Result<ListProductResult>>;
