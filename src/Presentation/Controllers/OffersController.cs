using Application.Offers.CreateBundleDiscountOffer;
using Contracts.Offers;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers.Common;

namespace Presentation.Controllers;

[Route("api/offers")]
public sealed class OffersController(ISender sender, IMapper mapper) : ApiController
{
    private readonly ISender _sender = sender;
    private readonly IMapper _mapper = mapper;

    [HttpPost("bundle-discount")]
    public async Task<IActionResult> CreateBundleDiscountOffer(CreateBundleDiscountOfferRequest request)
    {
        await Task.CompletedTask;
        var result =
            await _sender.Send(_mapper.Map<CreateBundleDiscountOfferCommand>(request));

        return result.Match(Ok, Problem);
    }
}
