using Domain.Orders.Enums;
using SharedKernel.Primitives;

namespace Domain.Orders.Errors;

public static partial class DomainError
{
    public static class Order
    {
        public static Error NotFound =
            Error.NotFound("Order.NotFound", "The order doesn't exists.");

        public static Error InvalidStatus(OrderStatus status) =>
            Error.Validation("Order.InvalidStatus", $"Can't operate on the orders the status is {status}.");
    }

    public static class LineItem
    {
        public static Error ExceedsAvailableQuantity(Guid id) => Error.Validation(
            "LineItem.ExceedsAvailableQuantity",
            $"Requested quantity to remove of product {id} exceeds the available quantity in order.");
    }
}
