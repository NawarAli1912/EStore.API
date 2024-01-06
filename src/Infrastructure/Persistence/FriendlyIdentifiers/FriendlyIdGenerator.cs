using Application.Common.FriendlyIdentifiers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.FriendlyIdentifiers;
internal class FriendlyIdGenerator(ApplicationDbContext dbContext)
    : IFriendlyIdGenerator
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<List<string>> GenerateOrderFriendlyId(int count)
    {

        List<string> ids = [];
        var sequence = await _dbContext.FriendlyIdSequences.SingleAsync();

        for (int i = 0; i < count; ++i)
        {
            ids.Add($"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{++sequence.OrderSequence}");

        }

        await _dbContext.SaveChangesAsync();
        return ids;
    }

    public async Task<List<string>> GenerateProductFriendlyId(int count)
    {
        List<string> ids = [];
        var sequence = await _dbContext.FriendlyIdSequences.SingleAsync();

        for (int i = 0; i < count; ++i)
        {

            ids.Add($"PRO-{DateTime.UtcNow:yyyyMMddHHmmss}-{++sequence.ProductSequence}");
        }

        await _dbContext.SaveChangesAsync();

        return ids;
    }
}
