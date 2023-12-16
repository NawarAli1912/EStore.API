using SharedKernel;

namespace Domain.Customers.Errors;

public static partial class DomainError
{
    public static class CartItem
    {
        public static Error NegativeQuantity = Error.Conflict(
            "CartItem.NegativeQuantity",
            "Invalid Quantity"
            );

        public static Error NotFound = Error.NotFound(
            "CartItem.NotFound",
            "This item doesn't exists");
    }
}
