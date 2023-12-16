using Domain.Orders;
using MediatR;
using SharedKernel;

namespace Application.Orders.GetCustomerOrders;

public record GetCustomerOrdersQuery(Guid CustomerId)
    : IRequest<Result<List<Order>>>;
