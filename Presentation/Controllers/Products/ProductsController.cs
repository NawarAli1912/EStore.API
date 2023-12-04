using Application.Common.Authentication.Models;
using Application.Products.Create;
using Application.Products.Filters;
using Application.Products.Get;
using Application.Products.List;
using Application.Products.ListByCategory;
using Contracts.Products;
using Infrastructure.Authentication;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Common.Models.Paging;
using Presentation.Controllers.Base;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace Presentation.Controllers.Products;

[Route("products")]
public class ProductsController(
    ISender sender,
    IMapper mapper) : ApiController
{
    private readonly ISender _sender = sender;
    private readonly IMapper _mapper = mapper;

    [HttpPost]
    [HasPermission(Permissions.CreateProduct)]
    public async Task<IActionResult> Create(CreateProductRequest request)
    {
        var result = await _sender.Send(_mapper.Map<CreateProductCommand>(request));

        return result.Match(
            value => CreatedAtAction(
                nameof(Get),
                new { id = value.Id },
                _mapper.Map<CreateProductResponse>(value)),
           Problem);
    }

    [HttpGet("{id:guid}", Name = "Get")]
    public async Task<IActionResult> Get(Guid id)
    {
        var productResult = await _sender.Send(new GetProductQuery(id));

        if (productResult.IsError)
        {
            return Problem(productResult.Errors);
        }

        if (User.IsInRole(Roles.Admin.ToString()))
        {
            return Ok(_mapper.Map<ProductDetailedResponse>(productResult.Value));
        }

        return Ok(_mapper.Map<ProductResponse>(productResult.Value));
    }

    [HttpGet]
    [HasPermission(Permissions.ReadDetails)]
    public async Task<IActionResult> List(
        [FromQuery] ListProductFilter filter,
        string? sortColumn,
        string? sortOrder,
        int page = 1,
        int pageSize = 10)
    {
        var productResult = await _sender.Send(new ListProductsQuery(
            filter,
            sortColumn,
            sortOrder,
            page,
            pageSize));

        if (productResult.IsError)
        {
            return Problem(productResult.Errors);
        }

        var result = productResult.Value;

        if (User.IsInRole(Roles.Admin.ToString()))
        {
            return Ok(PagedList<ProductDetailedResponse>.Create(
                            _mapper.Map<List<ProductDetailedResponse>>(result.Products),
                            page,
                            pageSize,
                            result.TotalCount));
        }


        return Ok(PagedList<ProductResponse>.Create(
                    _mapper.Map<List<ProductResponse>>(result.Products),
                    page,
                    pageSize,
                    result.TotalCount));
    }

    [HttpGet("category/{categoryId:guid}")]
    public async Task<IActionResult> ListByCategory(Guid categoryId)
    {
        var productsResult =
            await _sender.Send(new ListByCategoryQuery(categoryId));

        return productsResult.Match(
            value => Ok(value.Products),
            errors => Problem(errors));
    }

}
