using SharedKernel.Primitives;

namespace Domain.Errors;

public static partial class DomainError
{
    public static class Offers
    {
        public static Error NotFound =>
            Error.Validation("Offer.NotFound", "Offer doesn't exisits.");

        public static Error InvalidState(string name) =>
            Error.Unexpected("Offer.InvalidState", $"The offer {name}, is on an invalid state.");

        public static Error UnderAnotherOffer =>
            Error.Validation("Offer.UnderAnotherOffer", "Some products are already under another offer.");

        public static Error UnspportedProducts =>
            Error.Validation("Offer.UnspportedProducts", "Some products are not supported to be under an offer.");

    }
}
