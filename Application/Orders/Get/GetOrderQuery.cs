using Domain.Kernal;
using Domain.Orders;
using MediatR;

namespace Application.Orders.Get;

public record GetOrderQuery(Guid Id) : IRequest<Result<Order>>;
