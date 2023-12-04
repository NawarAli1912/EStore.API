using Application.Authentication.Common;
using Application.Common.Authentication.Jwt;
using Domain.DomainErrors.Authentication;
using Domain.Kernal;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Authentication.Login;

internal class LoginQueryHandler(
    UserManager<IdentityUser> userManager,
    IJwtTokenGenerator jwtTokenGenerator) :
    IRequestHandler<LoginQuery, Result<AuthenticationResult>>
{

    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;

    public async Task<Result<AuthenticationResult>> Handle(
        LoginQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Errors.Authentication.InvalidCredentials;
        }

        var token = await _jwtTokenGenerator.Generate(user);

        return new AuthenticationResult(
            user.Id,
            user.UserName ?? string.Empty,
            user.Email ?? string.Empty,
            token);
    }
}
