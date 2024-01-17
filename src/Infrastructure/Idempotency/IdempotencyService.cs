using Application.Common.Idempotency;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Idempotency;
internal sealed class IdempotencyService : IIdemptencyService
{
    private readonly ApplicationDbContext _context;

    public IdempotencyService(ApplicationDbContext context)
    {
        _context = context;
    }

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
