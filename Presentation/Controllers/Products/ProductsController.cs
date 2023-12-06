using Application.Common.Authentication.Models;
using Application.Products.Create;
using Application.Products.Delete;
using Application.Products.Filters;
using Application.Products.Get;
using Application.Products.List;
using Application.Products.UpdateBasicInfor;
using Contracts.Products;
using Infrastructure.Authentication;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Common.Models.Paging;
using Presentation.Controllers.Base;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace Presentation.Controllers.Products;

[Route("api/products")]
public class ProductsController(
    ISender sender,
    IMapper mapper) : ApiController
{
    private readonly ISender _sender = sender;
    private readonly IMapper _mapper = mapper;

    [HttpGet("{id:guid}", Name = "Get")]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await _sender.Send(new GetProductQuery(id));

        return result.Match(
            value => Ok(_mapper.Map<ProductResponse>(value)),
            Problem);
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] ListProductFilter filter,
        string? sortColumn,
        string? sortOrder,
        int page = 1,
        int pageSize = 10)
    {
        var result = await _sender.Send(new ListProductsQuery(
            filter,
            sortColumn,
            sortOrder,
            page,
            pageSize));

        return result.Match(
            value => Ok(PagedList<ProductResponse>.Create(
                                    _mapper.Map<List<ProductResponse>>(value.Products),
                                    page,
                                    pageSize,
                                    value.TotalCount)),
                            Problem);
    }


    [HttpPost]
    [HasPermission(Permissions.ManageProducts)]
    public async Task<IActionResult> Create(CreateProductsRequest request)
    {
        var result = await _sender.Send(_mapper.Map<CreateProductsCommand>(request));

        return result.Match(
            value => Ok(_mapper.Map<CreateProductsResponse>(value)),
            Problem);
    }

    [HttpGet("details")]
    [HasPermission(Permissions.ReadDetails)]
    public async Task<IActionResult> ListDetails(
        [FromQuery] ListProductFilter filter,
        string? sortColumn,
        string? sortOrder,
        int page = 1,
        int pageSize = 10)
    {
        var result = await _sender.Send(new ListProductsQuery(
            filter,
            sortColumn,
            sortOrder,
            page,
            pageSize));

        return result.Match(
            value => Ok(PagedList<ProductDetailedResponse>.Create(
                                    _mapper.Map<List<ProductDetailedResponse>>(value.Products),
                                    page,
                                    pageSize,
                                    value.TotalCount)),
                            Problem);
    }

    [HttpGet("details/{id:guid}", Name = "GetDetailes")]
    [HasPermission(Permissions.ReadDetails)]
    public async Task<IActionResult> GetDetails(Guid id)
    {
        var result = await _sender.Send(new GetProductQuery(id));

        return result.Match(
            value => Ok(_mapper.Map<ProductDetailedResponse>(value)),
            Problem);
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(Permissions.ManageProducts)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _sender.Send(new DeleteProductCommand(id));

        return result.Match(
            value => Ok(),
            Problem);
    }

    [HttpPatch("{id:guid}")]
    [HasPermission(Permissions.ManageProducts)]
    public async Task<IActionResult> Update(Guid id, UpdateProductBasicInfoRequest request)
    {
        var result = await _sender.Send(_mapper.Map<UpdateProductBasicInfoCommand>((id, request)));

        return result.Match(
            value => Ok(value),
            Problem);
    }
}
