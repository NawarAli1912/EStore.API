using Application.Products.Create;
using Application.Products.Get;
using Contracts.Products;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
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
        var product = await _sender.Send(new GetProductQuery(id));

        return product.Match(
            Ok,
            Problem);
    }
}
