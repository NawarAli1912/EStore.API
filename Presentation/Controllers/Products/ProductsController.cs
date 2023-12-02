﻿using Application.Common.Authentication;
using Application.Products.Create;
using Application.Products.Filters;
using Application.Products.Get;
using Application.Products.List;
using Contracts.Products;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = Roles.Admin)]
    [Produces(typeof(CreateProductResponse))]
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

    [HttpGet("{id}", Name = "Get")]

    public async Task<IActionResult> Get(Guid id)
    {
        var productResult = await _sender.Send(new GetProductQuery(id));

        if (productResult.IsError)
        {
            return Problem(productResult.Errors);
        }

        if (User.IsInRole(Roles.Admin))
        {
            return Ok(_mapper.Map<ProductDetailedResponse>(productResult.Value));
        }

        return Ok(_mapper.Map<ProductResponse>(productResult.Value));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ProductDetailedResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
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

        if (User.IsInRole(Roles.Admin))
        {
            return Ok(PagedList<ProductResponse>.Create(
                            _mapper.Map<List<ProductResponse>>(result.Products),
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
}
