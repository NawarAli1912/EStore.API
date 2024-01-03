using Application.Common.Repository;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace Infrastructure.BackgroundJobs;
internal class ProductOffersJob(ApplicationDbContext dbContext, IOffersRepository offersRepository) : IJob
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly IOffersRepository _offersRepository = offersRepository;

    public async Task Execute(IJobExecutionContext context)
    {
        var query = _dbContext
            .Products
            .Where(p => p.AssociatedOffers.Any())
            .OrderBy(p => p.Id);

        var percentageOffersDict = (await _offersRepository.ListPercentageDiscountOffers())!
            .ToDictionary(o => o.Id, o => o.Status);

        var bundleOffersDict = (await _offersRepository.ListBundleDiscountOffers())!
            .ToDictionary(o => o.Id, o => o.Status);


        int batchSize = 100;
        int totalCount = await query.CountAsync();
        int batches = (int)Math.Ceiling((double)totalCount / batchSize);

        for (int i = 0; i < batches; ++i)
        {
            var products = await query
                .Skip(i * batchSize)
                .Take(batchSize)
                .ToListAsync();

            foreach (var product in products)
            {
                foreach (var offerId in product.AssociatedOffers)
                {
                    if (bundleOffersDict.TryGetValue(offerId, out var status))
                    {
                        if (status == Domain.Offers.Enums.OfferStatus.Expired)
                        {
                            product.UnassociateOffer(offerId);
                        }
                    }

                    if (percentageOffersDict.TryGetValue(offerId, out var offerStatus))
                    {
                        if (status == Domain.Offers.Enums.OfferStatus.Expired)
                        {
                            product.UnassociateOffer(offerId);
                        }
                    }
                }

                _dbContext.Update(product);
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
