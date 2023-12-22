using Domain.Orders;
using MediatR;
using SharedKernel.Primitives;

namespace Application.Orders.Get;

public record GetOrderQuery(Guid Id) : IRequest<Result<Order>>;
