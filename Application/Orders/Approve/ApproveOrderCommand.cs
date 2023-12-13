using Domain.Kernal;
using MediatR;

namespace Application.Orders.Approve;

public record ApproveOrderCommand(Guid Id)
    : IRequest<Result<Updated>>;
