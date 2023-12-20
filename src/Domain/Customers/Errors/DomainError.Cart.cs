using SharedKernel;

namespace Domain.Customers.Errors;

public static partial class DomainError
{
    public static class Cart
    {
        public static Error EmptyCart = Error.Validation(
            "Cart.Empty",
            "The cart is emtpty.");
    }
}