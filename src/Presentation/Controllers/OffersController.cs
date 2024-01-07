using Application.Offers.CreateBundleDiscountOffer;
using Application.Offers.CreatePercentageDiscountOffer;
using Application.Offers.List;
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
        var result =
            await _sender.Send(_mapper.Map<CreateBundleDiscountOfferCommand>(request));

        return result
            .Match(
            _ => Created(),
            Problem);
    }

    [HttpPost("percentage-discount")]
    public async Task<IActionResult> CreatePercentageDiscountOffer(CreatePercentageDiscountOfferRequest request)
    {
        var result =
            await _sender.Send(_mapper.Map<CreatePercentageDiscountOfferCommand>(request));

        return result.Match(
            _ => Created(),
            Problem);
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var result = await _sender.Send(new ListOffersQuery());

        return result.Match(
            value => Ok(_mapper.Map<ListOfferResponse>(value)),
            Problem);
    }
}
