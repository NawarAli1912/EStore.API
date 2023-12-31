using Application.Common.Idempotency;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Idempotency;
internal class IdempotencyService(ApplicationDbContext context) : IIdemptencyService
{
    private readonly ApplicationDbContext _context = context;

    public async Task CreateRequest(Guid requestId, string name)
    {
        var idempotentRequest = new IdempotentRequest
        {
            Id = requestId,
            Name = name,
            CreatedAtUtc = DateTime.UtcNow,
        };

        await _context.Set<IdempotentRequest>().AddAsync(idempotentRequest);

        await _context.SaveChangesAsync();
    }

    public Task<bool> RequestExists(Guid requestId)
    {
        return _context.Set<IdempotentRequest>().AnyAsync(r => r.Id == requestId);
    }
}
