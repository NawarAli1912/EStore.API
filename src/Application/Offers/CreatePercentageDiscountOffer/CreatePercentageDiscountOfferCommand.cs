﻿using Domain.Offers;
using MediatR;
using SharedKernel.Primitives;

namespace Application.Offers.CreatePercentageDiscountOffer;

public record CreatePercentageDiscountOfferCommand(
    string Name,
    string Description,
    Guid ProductId,
    decimal Discount,
    DateOnly StartDate,
    DateOnly EndDate
    ) : IRequest<Result<PercentageDiscountOffer>>;
