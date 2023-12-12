using Application.Products.List;
using Domain.Kernal;
using MediatR;

namespace Application.Products.ListByCategory;

public record ListByCategoryQuery(
    Guid CategoryId,
    int Page = 1,
    int PageSize = 10) : IRequest<Result<ListProductResult>>;
