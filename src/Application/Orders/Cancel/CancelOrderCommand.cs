using SharedKernel.Messaging;
using SharedKernel.Primitives;

namespace Application.Orders.Cancel;
public record CancelOrderCommand(Guid Id)
    : IRequest<Result<Updated>>;
