using SharedKernel.Messaging;
using SharedKernel.Primitives;

namespace Application.Carts.Clear;

public record ClearCartCommand(Guid CustomerId)
    : IRequest<Result<Updated>>;
