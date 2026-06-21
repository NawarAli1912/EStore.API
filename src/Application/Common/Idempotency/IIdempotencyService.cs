namespace Application.Common.Idempotency;

public interface IIdempotencyService
{
    Task<bool> RequestExists(Guid requestId);

    Task CreateRequest(Guid requestId, string name);
}
