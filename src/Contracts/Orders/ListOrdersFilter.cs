namespace Contracts.Orders;

public record ListOrdersFilter(
    IEnumerable<OrderStatus>? Status,
    DateTime? ModifiedFrom,
    DateTime? ModifiedTo);
