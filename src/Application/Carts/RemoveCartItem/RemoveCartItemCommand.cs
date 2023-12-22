using Application.Carts.Common;
using MediatR;
using SharedKernel.Primitives;

namespace Application.Carts.RemoveCartItem;

public record RemoveCartItemCommand(
    Guid CustomerId,
    Guid ProductId,
    int Quantity) : IRequest<Result<AddRemoveCartItemResult>>;
