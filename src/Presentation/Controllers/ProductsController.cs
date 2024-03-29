﻿using Application.Common.Authentication.Models;
using Application.Products.AssignCategories;
using Application.Products.Create;
using Application.Products.Delete;
using Application.Products.Get;
using Application.Products.List;
using Application.Products.ListByCategory;
using Application.Products.ListUncategorizedProducts;
using Application.Products.UnassignCategories;
using Application.Products.Update;
using Contracts.Products;
using Infrastructure.Authentication.Authorization;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Common.Models.Paging;
using Presentation.Controllers.Common;
using SharedKernel.Primitives;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace Presentation.Controllers;

[Route("api/products")]
public sealed class ProductsController(
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
        int page = 1,
        int pageSize = 10)
    {
        var result = await _sender.Send(new ListProductsQuery(
            _mapper.Map<ProductsFilter>(filter),
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
    public async Task<IActionResult> Create(
        [FromHeader(Name = "X-Idempotency-Key")] string requestId,
        CreateProductsRequest request)
    {
        if (!Guid.TryParse(requestId, out Guid parsedRequestId))
        {
            return Problem([Error.Validation("IdempotencyKey.Failure", "IdempotencyKey must be guid.")]);
        }

        var command = _mapper.Map<CreateProductsCommand>((parsedRequestId, request));
        var result = await _sender.Send(command);

        return result.Match(_ => Created(), Problem);
    }

    [HttpGet("details")]
    [HasPermission(Permissions.ReadDetails)]
    public async Task<IActionResult> ListDetails(
        [FromQuery] ListProductsDetailsFilter filter,
        int page = 1,
        int pageSize = 10)
    {
        var result = await _sender.Send(new ListProductsQuery(
            _mapper.Map<ProductsFilter>(filter),
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

    [HttpGet("{id:guid}/details", Name = "GetDetailes")]
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
            _ => Ok(),
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
            _ => Ok(),
            Problem);
    }

    [HttpPatch("{id:guid}/categories/assign")]
    [HasPermission(Permissions.ManageProducts | Permissions.ManageCategories)]
    public async Task<IActionResult> AssignCategories(
        Guid id,
        AssignUnAssignCategoriesRequest request)
    {
        var result = await _sender.Send(_mapper.Map<AssignCategoriesCommand>((id, request)));

        return result.Match(
            _ => Ok(),
            Problem);
    }

    [HttpPatch("{id:guid}/categories/unassign")]
    [HasPermission(Permissions.ManageProducts | Permissions.ManageCategories)]
    public async Task<IActionResult> UnassignCategories(
        Guid id,
        AssignUnAssignCategoriesRequest request)
    {
        var result = await _sender.Send(
            _mapper.Map<UnassignCategoriesCommand>((id, request)));

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
            value => Ok(PagedList<ProductResponse>.Create(
                                    _mapper.Map<List<ProductResponse>>(value.Products),
                                    page,
                                    pageSize,
                                    value.TotalCount)),
            Problem);
    }

    [HttpGet("categories/uncategorized")]
    [HasPermission(Permissions.ManageProducts)]
    public async Task<IActionResult> ListUncategoriezedProducts(
        int page = 1,
        int pageSize = 10)
    {
        var result = await _sender
           .Send(new ListUncategorizedProductsQuery(page, pageSize));

        return result.Match(
            value => Ok(PagedList<ProductDetailedResponse>.Create(
                                    _mapper.Map<List<ProductDetailedResponse>>(value.Products),
                                    page,
                                    pageSize,
                                    value.TotalCount)),
            Problem);
    }

}
