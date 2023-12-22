using MediatR;
using SharedKernel.Primitives;

namespace Application.Carts.Get;
public record GetCartQuery(Guid CustomerId) : IRequest<Result<CartResult>>;
