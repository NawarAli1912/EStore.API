using Domain.Offers;

namespace Application.Common.Repository;

public interface IOffersRepository
{
    Task<List<Offer>?> List();
}
