using MediatR;
using SharedKernel;

namespace Application.Carts.Clear;

public record ClearCartCommand(Guid CustomerId)
    : IRequest<Result<Updated>>;
