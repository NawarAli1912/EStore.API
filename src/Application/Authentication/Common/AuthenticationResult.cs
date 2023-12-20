namespace Application.Authentication.Common;

public record AuthenticationResult(
    string Id,
    string UserName,
    string Email,
    string Token);

