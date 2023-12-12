using Domain.Kernal;

namespace Domain.DomainErrors;
public static partial class Errors
{
    public static class Cart
    {
        public static Error EmptyCart = Error.Validation(
            "Cart.Empty",
            "The cart is emtpty.");
    }
}