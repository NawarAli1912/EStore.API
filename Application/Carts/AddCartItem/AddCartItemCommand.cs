using Application.Carts.Common;
using MediatR;
using SharedKernel;

namespace Application.Carts.AddCartItem;

public record AddCartItemCommand(
    Guid CustomerId,
    Guid ProductId,
    int Quantity) : IRequest<Result<AddRemoveCartItemResult>>;
