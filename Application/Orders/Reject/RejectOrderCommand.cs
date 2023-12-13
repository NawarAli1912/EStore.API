using Domain.Kernal;
using MediatR;

namespace Application.Orders.Reject;

public record RejectOrderCommand(Guid Id) : IRequest<Result<Updated>>;
