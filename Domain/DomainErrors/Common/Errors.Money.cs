﻿using Domain.Kernal;

namespace Domain.DomainErrors.Common;
public static partial class Errors
{

    public static class Money
    {
        public static Error InvalidCurrency = Error.Validation("Money.InvalidCurrency", "Unspported currency provided.");

        public static Error InvalidAmount = Error.Validation("Money.InvalidAmount", "Money value is less than 0.");
    }
}

