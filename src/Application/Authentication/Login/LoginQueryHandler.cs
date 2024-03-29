﻿using Application.Authentication.Common;
using Application.Common.Authentication.Jwt;
using Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SharedKernel.Primitives;

namespace Application.Authentication.Login;

internal sealed class LoginQueryHandler :
    IRequestHandler<LoginQuery, Result<AuthenticationResult>>
{

    private readonly UserManager<IdentityUser> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginQueryHandler(
        UserManager<IdentityUser> userManager,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<AuthenticationResult>> Handle(
        LoginQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return DomainError.Authentication.InvalidCredentials;
        }

        var token = await _jwtTokenGenerator.Generate(user);

        return new AuthenticationResult(
            user.Id,
            user.UserName ?? string.Empty,
            user.Email ?? string.Empty,
            token);
    }
}
