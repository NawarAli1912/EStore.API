using Domain.Products;
using MediatR;
using SharedKernel;

namespace Application.Products.Get;

public sealed record GetProductQuery(
    Guid ProductId
    ) : IRequest<Result<Product>>;
