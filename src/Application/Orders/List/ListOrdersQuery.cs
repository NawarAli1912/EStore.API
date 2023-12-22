using MediatR;
using SharedKernel.Primitives;

namespace Application.Orders.List;
public record ListOrdersQuery(
    OrdersFilter Filter,
    int Page,
    int PageSize) : IRequest<Result<ListOrderResult>>;
