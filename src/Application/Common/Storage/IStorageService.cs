using Microsoft.AspNetCore.Http;

namespace Application.Common.Storage;
public interface IStorageService
{
    Task UploadFiles(Guid id, IEnumerable<IFormFile> files, FileSource source);

    Task DeleteFiles(List<string> keys, FileSource source);

    Task<List<File>> ListFilesLinks(Guid id, FileSource source);
}

public enum FileSource
{
    Product,
    Customer
}
