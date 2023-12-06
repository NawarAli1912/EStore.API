namespace Infrastructure;

public class ElasticSearchSettings
{
    public const string SectionName = "ElasticSearchSettings";

    public string BaseUrl { get; init; } = default!;

    public string DefaultIndex { get; init; } = default!;
}
