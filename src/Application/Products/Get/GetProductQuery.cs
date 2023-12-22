using Domain.Products;
using MediatR;
using SharedKernel.Primitives;

namespace Application.Products.Get;

public sealed record GetProductQuery(
    Guid ProductId
    ) : IRequest<Result<Product>>;
