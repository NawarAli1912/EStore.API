using Domain.Offers;

namespace Application.Common.Cache;

public interface IOffersStore
{
    Task<List<Offer>?> List();
}
