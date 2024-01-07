using Domain.Orders.Enums;
using SharedKernel.Primitives;

namespace Domain.Errors;

public static partial class DomainError
{
    public static class Orders
    {
        public static Error NotFound =
            Error.NotFound("Order.NotFound", "The order doesn't exists.");

        public static Error InvalidStatus(OrderStatus status) =>
            Error.Validation("Order.InvalidStatus", $"Can't operate on the orders the status is {status}.");

        public static Error EmptyLineItems => Error.Unexpected(
            "Order.EmptyLineItems", "Can't operate on empty order.");
    }

    public static class LineItem
    {
        public static Error ExceedsAvailableQuantity(Guid id) => Error.Validation(
            "LineItem.ExceedsAvailableQuantity",
            $"Requested quantity to remove of product {id} exceeds the available quantity in order.");

        public static Error InvalidCreationData => Error.Unexpected(
            "LineItem.InvalidCreationData", "Can't create order item with the provided data.");

        public static Error NotFound => Error.Validation(
            "LineItem.NotFound", "LineItem doesn't exists.");
    }
}
