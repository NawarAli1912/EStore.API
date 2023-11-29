namespace Contracts.Authentication;

public record RegisterRequest(
    string UserName,
    string Email,
    string Password,
    string? City,
    string? County,
    string? PostalCode,
    string? Building,
    string? Street);
