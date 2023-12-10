using Application.Carts.Common;
using Domain.Kernal;
using MediatR;

namespace Application.Carts.AddCartItem;

public record AddCartItemCommand(
    Guid CustomerId,
    Guid ProductId,
    int Quantity) : IRequest<Result<AddRemoveCartItemResult>>;
