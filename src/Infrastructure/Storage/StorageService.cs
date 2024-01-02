using Amazon.S3;
using Amazon.S3.Model;
using Application.Common.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Serilog;

namespace Infrastructure.Storage;

public sealed class StorageService(IAmazonS3 amazonS3, IOptions<StorageSettings> storageSettings) : IStorageService
{
    private readonly IAmazonS3 _amazonS3 = amazonS3;
    private readonly StorageSettings _storageSettings = storageSettings.Value;

    public async Task UploadFiles(Guid id, IEnumerable<IFormFile> files, FileSource source)
    {
        string bucketName = GetBuketName(source);
        var tasks = files.Select(file =>
        {
            PutObjectRequest request = new()
            {
                BucketName = bucketName,
                Key = $"poducts/{id}/{DateTime.UtcNow.Ticks}",
                ContentType = file.ContentType,
                InputStream = file.OpenReadStream(),
                Metadata =
                {
                    ["x-amz-meta-originalname"] = file.FileName,
                    ["x-amz-meta-extension"] = Path.GetExtension(file.FileName)
                }
            };

            return _amazonS3.PutObjectAsync(request);
        });

        await Task.WhenAll(tasks);
    }

    public async Task DeleteFiles(List<string> keys, FileSource source)
    {
        string bucketName = GetBuketName(source);

        var deleteTasks = keys.Select(k => Task.Run(() =>
        {
            var request = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = k
            };

            return _amazonS3.DeleteObjectAsync(request);
        }));

        await Task.WhenAll(deleteTasks);
    }

    public async Task<List<Image>> ListFilesLinks(Guid id, FileSource source)
    {
        string bucketName = GetBuketName(source);

        var listRequest = new ListObjectsV2Request
        {
            BucketName = bucketName,
            Prefix = $"poducts/{id}/"
        };

        var listResponse = await _amazonS3.ListObjectsV2Async(listRequest);


        var keys = listResponse
            .S3Objects
            .Select(item => item.Key)
            .ToList();

        Dictionary<string, Task<string>> keyToDownloadLinks = [];
        foreach (var key in keys)
        {
            keyToDownloadLinks.TryAdd(key, Task.Run(() =>
            {
                try
                {
                    var request = new GetPreSignedUrlRequest
                    {
                        Key = key,
                        BucketName = bucketName,
                        Expires = DateTime.UtcNow.Add(TimeSpan.FromHours(2)),
                        Protocol = Protocol.HTTPS
                    };

                    return _amazonS3.GetPreSignedURLAsync(request);

                }
                catch (Exception ex)
                {
                    Log.Error(ex, ex.Message);
                    return null;
                }
            }));
        }

        await Task.WhenAll(keyToDownloadLinks.Values);

        List<Image> result = [];
        foreach (var key in keys)
        {
            result.Add(new Image
            {
                Key = key,
                DownloadLink = keyToDownloadLinks[key].Result
            });
        }

        return result;
    }

    private string GetBuketName(FileSource source)
    {
        return source switch
        {
            FileSource.Product => _storageSettings.ProductsBucket,
            FileSource.Customer => _storageSettings.ProductsBucket,
            _ => throw new ArgumentException()
        };
    }
}
