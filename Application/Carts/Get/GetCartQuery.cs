using Domain.Kernal;
using MediatR;

namespace Application.Carts.Get;
public record GetCartQuery(Guid CustomerId) : IRequest<Result<CartResult>>;
