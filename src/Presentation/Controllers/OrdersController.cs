using Application.Common.Authentication.Models;
using Application.Orders.Approve;
using Application.Orders.Cancel;
using Application.Orders.Get;
using Application.Orders.GetCustomerOrders;
using Application.Orders.List;
using Application.Orders.Reject;
using Application.Orders.Update;
using Contracts.Orders;
using Infrastructure.Authentication.Authorization;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Common.Models.Paging;
using Presentation.Controllers.Common;

namespace Presentation.Controllers;

[Route("api/orders")]
public sealed class OrdersController(ISender sender, IMapper mapper) : ApiController
{
    private readonly ISender _sender = sender;
    private readonly IMapper _mapper = mapper;

    [HttpGet("customer/{customerId:guid}")]
    [HasPermission(Permissions.ManageOrders | Permissions.ManageOrdersLite)]
    public async Task<IActionResult> GetCustomerOrders(Guid customerId)
    {
        var result = await _sender.Send(
            new GetCustomerOrdersQuery(customerId));

        return result.Match(
            value => Ok(_mapper.Map<List<OrderResponse>>(value)),
            Problem);
    }

    [HttpGet("{id:guid}")]
    [HasPermission(Permissions.ManageOrders)]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await _sender.Send(new GetOrderQuery(id));

        return result.Match(
            value => Ok(_mapper.Map<OrderResponse>(value)),
            Problem);
    }

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
            value => Ok(PagedList<OrderResponse>.Create(
                _mapper.Map<List<OrderResponse>>(value.Orders),
                page,
                pageSize,
                value.TotalCount)),
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

    [HttpPatch("{id:guid}/update")]
    [HasPermission(Permissions.ManageOrdersLite)]
    public async Task<IActionResult> Update(Guid id, UpdateOrderRequest request)
    {
        _ = await _sender.Send(
            _mapper.Map<UpdateOrderCommand>((id, request)));

        return Ok();
    }

    [HttpPatch("{id:guid}/cancel")]
    [HasPermission(Permissions.ManageOrdersLite)]
    public async Task<IActionResult> Cancel(Guid id)
    {
        _ = await _sender.Send(new CancelOrderCommand(id));

        return Ok();
    }
}
