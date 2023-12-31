namespace Application.Common.Idempotency;

public interface IIdemptencyService
{
    Task<bool> RequestExists(Guid requestId);

    Task CreateRequest(Guid requestId, string name);
}
