using MediatR;
using SharedKernel.Primitives;

namespace Application.Orders.Cancel;
public record CancelOrderCommand(Guid Id) : IRequest<Result<Updated>>;
