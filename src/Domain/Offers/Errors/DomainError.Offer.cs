using SharedKernel.Primitives;

namespace Domain.Offers.Errors;

public static partial class DomainError
{
    public static class Offer
    {
        public static Error UnderAnotherOffer =>
            Error.Validation("Offer.UnderAnotherOffer", $"some products are already under another offer.");
    }
}
