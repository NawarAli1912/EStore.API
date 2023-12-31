﻿using MediatR;
using SharedKernel.Primitives;

namespace Application.Offers.List;
public record ListOffersQuery() : IRequest<Result<ListOfferResult>>;
