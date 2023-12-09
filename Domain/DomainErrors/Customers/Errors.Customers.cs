using Domain.Kernal;

namespace Domain.DomainErrors;

public static partial class Errors
{
    public static class Customers
    {
        public static Error DuplicateEmail = Error.Conflict(
            "Cusotmer.DuplicateEmail",
            "Email already exists.");
    }
}
