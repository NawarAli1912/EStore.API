using Domain.Offers;

namespace Application.Common.Repository;

public interface IOffersRepository
{
    Task<List<BundleDiscountOffer>?> ListBundleDiscountOffers();

    Task<List<PercentageDiscountOffer>?> ListPercentageDiscountOffers();
}
