using Domain.Kernal;

namespace Domain.DomainErrors.Common;
public static partial class Errors
{

    public static class Money
    {
        public static Error InvalidCurrency = Error.Validation("Money.InvalidCurrency", "Unspported currency provided.");
    }
}

