using Domain.Orders;
using SharedKernel.Messaging;
using SharedKernel.Primitives;

namespace Application.Orders.Get;

public record GetOrderQuery(Guid Id) : IRequest<Result<Order>>;
