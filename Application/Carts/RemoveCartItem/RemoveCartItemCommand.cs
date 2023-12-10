using Application.Carts.Common;
using Domain.Kernal;
using MediatR;

namespace Application.Carts.RemoveCartItem;

public record RemoveCartItemCommand(
    Guid CustomerId,
    Guid ProductId,
    int Quantity) : IRequest<Result<AddRemoveCartItemResult>>;
