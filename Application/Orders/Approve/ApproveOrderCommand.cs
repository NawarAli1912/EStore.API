using MediatR;
using SharedKernel;

namespace Application.Orders.Approve;

public record ApproveOrderCommand(Guid Id)
    : IRequest<Result<Updated>>;
