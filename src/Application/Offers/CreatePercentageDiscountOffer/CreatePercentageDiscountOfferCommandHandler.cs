using Application.Common.Data;
using Domain.Offers;
using MediatR;
using SharedKernel.Primitives;

namespace Application.Offers.CreatePercentageDiscountOffer;

internal sealed class CreatePercentageDiscountOfferCommandHandler(IApplicationDbContext context)
        : IRequestHandler<CreatePercentageDiscountOfferCommand, Result<PercentageDiscountOffer>>
{
    private readonly IApplicationDbContext _context = context;

    public Task<Result<PercentageDiscountOffer>> Handle(CreatePercentageDiscountOfferCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
