using Domain.Orders;
using MediatR;
using SharedKernel;

namespace Application.Orders.Get;

public record GetOrderQuery(Guid Id) : IRequest<Result<Order>>;
