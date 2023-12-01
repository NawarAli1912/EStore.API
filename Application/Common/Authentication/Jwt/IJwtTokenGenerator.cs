using Microsoft.AspNetCore.Identity;

namespace Application.Common.Authentication.Jwt;

public interface IJwtTokenGenerator
{
    string Generate(IdentityUser user, IList<string> roles);
}
