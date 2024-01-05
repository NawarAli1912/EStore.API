using Application.Common.Repository;
using Domain.Offers;
using MediatR;
using SharedKernel.Primitives;

namespace Application.Offers.List;
internal sealed class ListOffersQueryHandler(IOffersRepository offersRepository) : IRequestHandler<ListOffersQuery, Result<List<Offer>>>
{
    private readonly IOffersRepository _offersRepository = offersRepository;

    public async Task<Result<List<Offer>>> Handle(ListOffersQuery request, CancellationToken cancellationToken)
    {
        var offers = await _offersRepository.List();

        return offers!;
    }
}
