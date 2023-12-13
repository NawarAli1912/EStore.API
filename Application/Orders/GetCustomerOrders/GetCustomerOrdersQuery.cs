using Domain.Kernal;
using Domain.Orders;
using MediatR;

namespace Application.Orders.GetCustomerOrders;

public record GetCustomerOrdersQuery(Guid CutomerId)
    : IRequest<Result<List<Order>>>;
