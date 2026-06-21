using Application.Carts.Common;
using SharedKernel.Messaging;
using SharedKernel.Primitives;

namespace Application.Carts.RemoveCartItem;

public record RemoveCartItemCommand(
    Guid CustomerId,
    Guid ItemId,
    int Quantity) : IRequest<Result<AddRemoveCartItemResult>>;
