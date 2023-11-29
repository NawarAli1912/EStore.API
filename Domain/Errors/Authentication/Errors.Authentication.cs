using Domain.Kernal;

namespace Domain.Errors.Authentication;

public static partial class Errors
{
    public static class Authentication
    {
        public static Error InvalidCredentials = Error.Conflict(
            "Auth.InvalidCredentials",
            "Invalid email or password.");
    }
}
