using SharedKernel;

namespace Domain.Customers.Errors;

public static partial class DomainError
{
    public static class Cart
    {
        public static Error EmptyCart = Error.Validation(
            "Cart.Empty",
            "The cart is emtpty.");


        public static Error CheckoutFailed = Error.Unexpected(
            "Cart.CheckoutFailed",
            "The checkout process has encountered unexpeted error.");
    }
}