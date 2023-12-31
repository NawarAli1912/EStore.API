namespace Infrastructure.Idempotency;
public sealed class IdempotentRequest
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }
}
