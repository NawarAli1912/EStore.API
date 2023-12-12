using Application.Carts.AddCartItem;
using Application.Carts.Checkout;
using Application.Carts.Clear;
using Application.Carts.Get;
using Application.Carts.RemoveCartItem;
using Application.Common.Authentication.Models;
using Contracts.Carts;
using Infrastructure.Authentication;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers.Base;
using System.Security.Claims;

namespace Presentation.Controllers;


[Route("api/carts")]
public class CartsContorller(ISender sender, IMapper mapper) : ApiController
{
    private readonly ISender _sender = sender;
    private readonly IMapper _mapper = mapper;

    [HttpGet]
    [HasPermission(Permissions.ManageCarts)]
    public async Task<IActionResult> Get()
    {
        string customerId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var result = await _sender.Send(new GetCartQuery(Guid.Parse(customerId)));

        return result.Match(
            value => Ok(_mapper.Map<CartResponse>(value)),
            Problem);
    }

    [HttpPatch("add-item")]
    [HasPermission(Permissions.ManageCarts)]
    public async Task<IActionResult> AddItem(AddRemoveCartItemRequest request)
    {
        string customerId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        var result = await _sender.Send(
            _mapper.Map<AddCartItemCommand>((Guid.Parse(customerId), request)));

        return result.Match(
            value => Ok(_mapper.Map<AddRemoveCartItemResponse>(value)),
            Problem);

    }


    [HttpPatch("remove-item")]
    [HasPermission(Permissions.ManageCarts)]
    public async Task<IActionResult> RemoveItem(AddRemoveCartItemRequest request)
    {
        string customerId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        var result = await _sender.Send(
            _mapper.Map<RemoveCartItemCommand>((Guid.Parse(customerId), request)));

        return result.Match(
            value => Ok(_mapper.Map<AddRemoveCartItemResponse>(value)),
            Problem);

    }

    [HttpDelete("clear")]
    [HasPermission(Permissions.ManageCarts)]
    public async Task<IActionResult> Clear()
    {
        string customerId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        var result = await _sender.Send(
            new ClearCartCommand(Guid.Parse(customerId)));

        return result.Match(
            _ => Ok(),
            Problem);
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout(CheckoutRequest request)
    {
        string customerId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var result = await _sender
            .Send(_mapper.Map<CheckoutCommand>((Guid.Parse(customerId), request)));

        return result.Match(
            value => Ok(value),
            Problem);
    }
}
