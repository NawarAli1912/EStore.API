using Application.Authentication.Login;
using Application.Authentication.Register;
using Contracts.Authentication;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers.Base;
using LoginRequest = Contracts.Authentication.LoginRequest;
using RegisterRequest = Contracts.Authentication.RegisterRequest;

namespace Presentation.Controllers;

public class AuthenticationController(
    ISender sender,
    IMapper mapper) : ApiController
{
    private readonly ISender _sender = sender;
    private readonly IMapper _mapper = mapper;

    [HttpPost("register")]
    public async Task<IActionResult> Create(RegisterRequest request)
    {
        var result = await _sender.Send(_mapper.Map<RegisterCommand>(request));

        return result.Match(
            value => Ok(_mapper.Map<AuthenticationResponse>(value)),
            Problem);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _sender.Send(_mapper.Map<LoginQuery>(request));

        return result.Match(
            value => Ok(_mapper.Map<AuthenticationResponse>(value)),
            Problem);
    }
}
