using Application.Common.Authentication.Models;
using Application.Products.AssignCategories;
using Application.Products.Create;
using Application.Products.Delete;
using Application.Products.Filters;
using Application.Products.Get;
using Application.Products.List;
using Application.Products.ListByCategory;
using Application.Products.Update;
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
        [FromQuery] ListProductsFilter filter,
        string? sortColumn,
        string? sortOrder,
        int page = 1,
        int pageSize = 10)
    {
        var result = await _sender.Send(new ListProductsQuery(
            _mapper.Map<ProductsFilter>(filter),
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
        [FromQuery] ListProductsDetailsFilter filter,
        string? sortColumn,
        string? sortOrder,
        int page = 1,
        int pageSize = 10)
    {
        var result = await _sender.Send(new ListProductsQuery(
            _mapper.Map<ProductsFilter>(filter),
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
    public async Task<IActionResult> Update(
        Guid id,
        UpdateProductRequest request)
    {
        var result = await _sender
            .Send(_mapper.Map<UpdateProductCommand>((id, request)));

        return result.Match(
            value => Ok(value),
            Problem);
    }

    [HttpPost("{id:guid}/categories")]
    [HasPermission(Permissions.ManageProducts | Permissions.ManageCategories)]
    public async Task<IActionResult> AssignCategories(
        Guid id,
        AssignCategoriesRequest request)
    {
        var result = await _sender.Send(_mapper.Map<AssignCategoriesCommand>((id, request)));

        return result.Match(
            _ => Ok(),
            Problem);
    }


    [HttpGet("categories/{categoryId:guid}")]
    public async Task<IActionResult> ListByCategory(
        Guid categoryId,
        int page = 1,
        int pageSize = 10)
    {
        var result = await _sender
            .Send(new ListByCategoryQuery(categoryId, page, pageSize));

        return result.Match(
            Ok,
            Problem);
    }
}
