using MediatR;
using SharedKernel;

namespace Application.Orders.Reject;

public record RejectOrderCommand(Guid Id) : IRequest<Result<Updated>>;
