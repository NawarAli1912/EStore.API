using SharedKernel.Messaging;
using SharedKernel.Primitives;

namespace Application.Orders.Reject;

public record RejectOrderCommand(Guid Id) : IRequest<Result<Updated>>;
