using MediatR;
using SharedKernel.Primitives;

namespace Application.Orders.Approve;

public record ApproveOrderCommand(Guid Id)
    : IRequest<Result<Updated>>;
