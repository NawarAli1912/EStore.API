using SharedKernel.Primitives;

namespace Domain.Authentication.Errors;

public static partial class DomainError
{
    public static class Authentication
    {
        public static Error InvalidCredentials = Error.Conflict(
            "Auth.InvalidCredentials",
            "Invalid email or password.");
    }
}
