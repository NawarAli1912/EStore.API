using Application.Common.Authentication.Models;
using Application.Orders.Approve;
using Application.Orders.List;
using Application.Orders.Reject;
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

    [HttpPatch("{id:guid}/approve")]
    [HasPermission(Permissions.ManageOrders)]
    public async Task<IActionResult> Approve(Guid id)
    {
        var result = await _sender.Send(new ApproveOrderCommand(id));

        return result.Match(
            _ => Ok(),
            Problem);
    }

    [HttpPatch("{id:guid}/reject")]
    [HasPermission(Permissions.ManageOrders)]
    public async Task<IActionResult> Reject(Guid id)
    {
        var result = await _sender.Send(new RejectOrderCommand(id));

        return result.Match(
            _ => Ok(),
            Problem);
    }

}
