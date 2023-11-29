using Application.Authentication.Register;
using Contracts.Authentication;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers.Base;

namespace Presentation.Controllers.Authentication;

[ApiController]
public class AuthenticationController : ApiController
{
    private readonly ISender _sender;

    public AuthenticationController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Create(RegisterRequest request)
    {
        var result = await _sender.Send(new RegisterCommand(
            request.UserName,
            request.Email,
            request.Password,
            request.Street,
            request.Building,
            request.City,
            request.County,
            request.PostalCode));

        return result.Match(
            value => Ok(value),
            errors => Problem(errors));
    }
}
