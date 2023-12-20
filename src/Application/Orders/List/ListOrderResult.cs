using Domain.Orders;

namespace Application.Orders.List;

public record ListOrderResult(List<Order> Orders, int TotalCount);
