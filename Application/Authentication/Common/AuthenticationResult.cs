namespace Application.Authentication.Common;

public record AuthenticationResult(
    Guid Id,
    string UserName,
    string Email,
    string Token);

