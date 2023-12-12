using Domain.Kernal;
using Domain.Orders;
using MediatR;

namespace Application.Orders.List;
public record ListOrdersQuery(
    OrdersFilter Filter,
    int Page,
    int PageSize) : IRequest<Result<List<Order>>>;
