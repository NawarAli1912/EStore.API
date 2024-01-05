using SharedKernel.Primitives;

namespace Domain.Errors;

public static partial class DomainError
{
    public static class Customers
    {
        public static Error DuplicateEmail = Error.Conflict(
            "Cusotmer.DuplicateEmail",
            "Email already exists.");

        public static Error NotFound = Error.NotFound(
            "Customer.NotFound",
            "Customer doesn't exists.");
    }
}
