namespace Infrastructure.Storage;
public class StorageSettings
{
    public const string SectionName = "StorageSettings";

    public string ProductsBucket { get; init; } = default!;

    public string CustomersBucket { get; init; } = default!;
}
