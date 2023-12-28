using Application.Offers.CreateBundleDiscountOffer;
using Contracts.Offers;
using Mapster;

namespace Presentation.Common.Mapping;

public class OffersMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateBundleDiscountOfferRequest, CreateBundleDiscountOfferCommand>();
    }
}
