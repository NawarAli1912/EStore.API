using Application.Authentication.Register;
using Contracts.Authentication;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers.Base;

namespace Presentation.Controllers.Authentication;

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
            errors => Problem(errors));
    }
}
