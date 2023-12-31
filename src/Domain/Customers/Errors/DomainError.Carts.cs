﻿using SharedKernel.Primitives;

namespace Domain.Errors;

public static partial class DomainError
{
    public static class Carts
    {
        public static Error EmptyCart = Error.Validation(
            "Cart.Empty",
            "The cart is emtpty.");


        public static Error CheckoutFailed = Error.Unexpected(
            "Cart.CheckoutFailed",
            "The checkout process has encountered unexpeted error.");
    }
}