﻿using Application.Categories.Create;
using Application.Categories.Delete;
using Application.Categories.GetFullHierarchy;
using Application.Categories.GetHierarchyDownward;
using Application.Categories.Update;
using Application.Common.Authentication.Models;
using Contracts.Categories;
using Infrastructure.Authentication.Authorization;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers.Common;
using SubcategoryActions = Contracts.Categories.SubcategoryActions;

namespace Presentation.Controllers;

[Route("api/categories")]
public sealed class CategoriesController(ISender sender, IMapper mapper) : ApiController
{
    private readonly ISender _sender = sender;
    private readonly IMapper _mapper = mapper;

    [HttpGet("hierarchy/all")]
    public async Task<IActionResult> GetHierarchy()
    {
        var result = await _sender.Send(new GetFullHierarchyQuery());

        return result.Match(
            value => Ok(_mapper.Map<List<CategoryResponse>>(value)),
            Problem
            );
    }

    [HttpGet("hierarchy/{id}")]
    public async Task<IActionResult> GetHierarchyDownward(Guid id)
    {
        var result = await _sender.Send(new GetHierarchyDownwardQuery(id));

        return result.Match(
            value => Ok(_mapper.Map<CategoryResponse>(value)),
            Problem);
    }

    [HttpPost]
    [HasPermission(Permissions.ManageCategories)]
    public async Task<IActionResult> Create(CreateCategoryRequest request)
    {
        var result = await _sender.Send(
            _mapper.Map<CreateCategoryCommand>(request));

        return result.Match(
            _ => Created(),
            Problem);
    }

    [HttpPatch("{id:guid}")]
    [HasPermission(Permissions.ManageCategories)]
    public async Task<IActionResult> Update(Guid id, UpdateCategoryRequest request)
    {
        var result = await _sender.Send(
            _mapper.Map<UpdateCategoryCommand>((id, request)));

        return result.Match(
            _ => Ok(),
            Problem);
    }

    [HttpDelete("{id:guid}")]
    [HasPermission(Permissions.ManageCategories)]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] SubcategoryActions action)
    {
        var result = await _sender.Send(new DeleteCategoryCommand(id, _mapper.Map<Domain.Categories.Enums.SubcategoryActions>(action)));

        return result.Match(
            _ => Ok(),
            Problem);

    }
}
