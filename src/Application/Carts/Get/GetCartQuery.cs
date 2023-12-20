using MediatR;
using SharedKernel;

namespace Application.Carts.Get;
public record GetCartQuery(Guid CustomerId) : IRequest<Result<CartResult>>;
