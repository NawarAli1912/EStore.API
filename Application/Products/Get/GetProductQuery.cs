using Domain.Kernal;
using Domain.Products;
using MediatR;

namespace Application.Products.Get;

public sealed record GetProductQuery(
    Guid ProductId
    ) : IRequest<Result<Product>>;
