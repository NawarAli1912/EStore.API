namespace Application.Common.Storage;
public sealed class File
{
    public string Key { get; set; } = default!;

    public string DownloadLink { get; set; } = default!;

    public byte[]? Data { get; set; }

    public string? ContentType { get; set; } = default!;

    public string? OriginalName { get; set; } = default!;
}
