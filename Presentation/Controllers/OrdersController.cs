using Application.Common.Authentication.Models;
using Application.Orders.List;
using Contracts.Orders;
using Infrastructure.Authentication;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers.Base;

namespace Presentation.Controllers;

[Route("api/orders")]
public class OrdersController(ISender sender, IMapper mapper) : ApiController
{
    private readonly ISender _sender = sender;
    private readonly IMapper _mapper = mapper;

    [HttpGet]
    [HasPermission(Permissions.ManageOrders)]
    public async Task<IActionResult> List(
        [FromQuery] ListOrdersFilter filter,
        int page = 1,
        int pageSize = 10)
    {
        var result = await _sender.Send(
            new ListOrdersQuery(
                _mapper.Map<OrdersFilter>(filter),
                page,
                pageSize));

        return result.Match(
            Ok,
            Problem);
    }
}
