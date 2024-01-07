using Application.Offers.CreateBundleDiscountOffer;
using Application.Offers.CreatePercentageDiscountOffer;
using Application.Offers.List;
using Contracts.Offers;
using Domain.Offers;
using Mapster;

namespace Presentation.Common.Mapping;

public class OffersMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateBundleDiscountOfferRequest,
            CreateBundleDiscountOfferCommand>();

        config.NewConfig<CreatePercentageDiscountOfferRequest,
            CreatePercentageDiscountOfferCommand>();

        config.NewConfig<ListOfferResult, ListOfferResponse>()
            .MapWith(src => ListOfferResultToListOfferResponse(src.Offers, src.OffersPriceDict));
    }

    private ListOfferResponse ListOfferResultToListOfferResponse(List<Offer> offers, Dictionary<Guid, decimal> offersPrices)
    {
        List<BundleDiscountOfferRespone> bundleOffer = [];
        List<PercentageDiscountOfferResponse> percentageOffers = [];

        foreach (Offer offer in offers)
        {
            if (offer.Type == Domain.Offers.Enums.OfferType.BundleDiscountOffer)
            {
                var parsedOffer = (BundleDiscountOffer)offer;
                bundleOffer.Add(new BundleDiscountOfferRespone(
                    parsedOffer.Id,
                    parsedOffer.Name,
                    parsedOffer.Description,
                    [.. parsedOffer.BundleProductsIds],
                    parsedOffer.Discount,
                    MapDomainOfferStatus(parsedOffer.Status),
                    parsedOffer.StartsAt,
                    parsedOffer.EndsAt,
                    offersPrices[parsedOffer.Id]));
            }
            else if (offer.Type == Domain.Offers.Enums.OfferType.PercentageDiscountOffer)
            {
                var parsedOffer = (PercentageDiscountOffer)offer;
                percentageOffers.Add(new PercentageDiscountOfferResponse(
                    parsedOffer.Id,
                    parsedOffer.Name,
                    parsedOffer.Description,
                    parsedOffer.ProductId,
                    parsedOffer.Discount,
                    MapDomainOfferStatus(parsedOffer.Status),
                    parsedOffer.StartsAt,
                    parsedOffer.EndsAt,
                    offersPrices[parsedOffer.Id]));
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        return new ListOfferResponse(percentageOffers, bundleOffer);
    }

    private OfferStatus MapDomainOfferStatus(Domain.Offers.Enums.OfferStatus status) => status switch
    {
        Domain.Offers.Enums.OfferStatus.Published => OfferStatus.Published,
        Domain.Offers.Enums.OfferStatus.Expired => OfferStatus.Expired,
        Domain.Offers.Enums.OfferStatus.Draft => OfferStatus.Draft,
        _ => throw new NotImplementedException()
    };
}
