using MediatR;
using SharedKernel;

namespace Application.Orders.List;
public record ListOrdersQuery(
    OrdersFilter Filter,
    int Page,
    int PageSize) : IRequest<Result<ListOrderResult>>;
