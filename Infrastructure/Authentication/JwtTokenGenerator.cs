using Application.Common.Authentication.Jwt;
using Infrastructure.Authentication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Authentication;

internal class JwtTokenGenerator(IOptions<JwtSettings> jwtSettings, IPermissionService permissionService)
    : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;
    private readonly IPermissionService _permissionService = permissionService;

    public async Task<string> Generate(
        IdentityUser user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.GivenName, user.UserName ?? string.Empty),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
        };

        var permission = await _permissionService.GetPermissions(user.Id);
        claims.Add(new(CustomClaims.Permissions, permission.ToString()));

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
            SecurityAlgorithms.HmacSha256);

        var securityToken = new JwtSecurityToken(
           issuer: _jwtSettings.Issuer,
           audience: _jwtSettings.Audience,
           expires: DateTime.Now.AddMinutes(_jwtSettings.ExpirationTimeInMinutes),
           claims: claims,
           signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }
}
