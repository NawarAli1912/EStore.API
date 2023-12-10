using Domain.Kernal;
using MediatR;

namespace Application.Carts.Clear;

public record ClearCartCommand(Guid CustomerId)
    : IRequest<Result<Updated>>;
