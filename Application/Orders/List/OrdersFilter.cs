using Domain.Orders.Enums;

namespace Application.Orders.List;
public record OrdersFilter(
    List<OrderStatus> Status,
    DateTime ModifiedFrom,
    DateTime ModifiedTo
    );
