using Microsoft.AspNetCore.Identity;

namespace Application.Common.Authentication.Jwt;

public interface IJwtTokenGenerator
{
    Task<string> Generate(IdentityUser user);
}
