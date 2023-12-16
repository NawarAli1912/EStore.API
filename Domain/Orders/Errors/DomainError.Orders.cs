using Domain.Orders.Enums;
using SharedKernel;

namespace Domain.Orders.Errors;
public static partial class DomainError
{
    public static class Orders
    {
        public static Error NotFound =
            Error.NotFound("Order.NotFound", "The order doesn't exists.");

        public static Error InvalidStatus(OrderStatus status) =>
            Error.Validation("Order.InvalidStatus", $"Can't operate on the orders the status is {status}.");
    }
}
