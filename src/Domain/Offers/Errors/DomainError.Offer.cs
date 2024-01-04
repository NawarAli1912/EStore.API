using SharedKernel.Primitives;

namespace Domain.Errors;

public static partial class DomainError
{
    public static class Offer
    {
        public static Error UnderAnotherOffer =>
            Error.Validation("Offer.UnderAnotherOffer", "Some products are already under another offer.");

        public static Error UnspportedProducts =>
            Error.Validation("Offer.UnspportedProducts", "Some products are not supported to be under an offer.");
    }
}
