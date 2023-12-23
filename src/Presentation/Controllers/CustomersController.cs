using Application.Common.Authentication.Models;
using Infrastructure.Authentication.Authorization;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers.Base;

namespace Presentation.Controllers;

[Route("api/customers")]
public sealed class CustomersController(ISender sender, IMapper mapper) : ApiController
{
    private readonly ISender _sender = sender;
    private readonly IMapper _mapper = mapper;

    [HttpGet]
    [HasPermission(Permissions.ManageCustomers)]
    public async Task<IActionResult> List(int page = 1, int pageSize = 10)
    {
        return Ok();
    }
}
