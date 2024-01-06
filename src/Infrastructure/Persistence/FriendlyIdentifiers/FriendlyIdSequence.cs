namespace Infrastructure.Persistence.FriendlyIdentifiers;

public sealed class FriendlyIdSequence
{
    public int Id { get; private set; } = 1;

    public int ProductSequence { get; set; }

    public int OrderSequence { get; set; }
}
