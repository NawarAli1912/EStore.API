using Application.Carts.Common;
using SharedKernel.Messaging;
using SharedKernel.Primitives;

namespace Application.Carts.AddCartItem;

public record AddCartItemCommand(
    Guid CustomerId,
    Guid? ProductId,
    Guid? OfferId,
    int Quantity) : IRequest<Result<AddRemoveCartItemResult>>;
