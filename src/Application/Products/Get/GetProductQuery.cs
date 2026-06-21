using Domain.Products;
using SharedKernel.Messaging;
using SharedKernel.Primitives;

namespace Application.Products.Get;

public sealed record GetProductQuery(
    Guid ProductId
    ) : IRequest<Result<Product>>;
