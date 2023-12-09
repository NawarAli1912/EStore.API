using Application.Categories.Create;
using Application.Categories.GetFullHierarchy;
using Application.Categories.GetHierarchyDownward;
using Contracts.Categories;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers.Base;

namespace Presentation.Controllers.Categories;

[Route("api/categories")]
public class CategoriesController(ISender sender, IMapper mapper) : ApiController
{
    private readonly ISender _sender = sender;
    private readonly IMapper _mapper = mapper;

    [HttpGet("hierarchy/all")]
    public async Task<IActionResult> GetHierarchy()
    {
        var result = await _sender.Send(new GetFullHierarchyQuery());

        return Ok(result);
    }

    [HttpGet("hierarchy/{id}")]
    public async Task<IActionResult> GetHierarchyDownward(Guid id)
    {
        var result = await _sender.Send(new GetHierarchyDownwardQuery(id));

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCategoryRequest request)
    {
        var result = await _sender.Send(_mapper.Map<CreateCategoryCommand>(request));

        return result.Match(
            _ => Ok(),
            Problem);
    }
}
