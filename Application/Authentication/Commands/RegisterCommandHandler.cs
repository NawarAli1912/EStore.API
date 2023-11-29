using Application.Common.Authentication.Jwt;
using Domain.Kernal;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Authentication.Commands;

internal class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<string>>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RegisterCommandHandler(UserManager<IdentityUser> userManager, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<string>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var user = new IdentityUser
        {
            UserName = request.UserName,
            Email = request.Email,
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        return _jwtTokenGenerator.Generate(user.Id, user.UserName, user.Email);
    }
}
