using Application.Common.Storage;
using Contracts.Products;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers.Common;

namespace Presentation.Controllers;


[ApiController]
[Route("api/products")]
public class ProductsImagesController(IStorageService storageService) : ApiController
{
    private readonly IStorageService _storageService = storageService;

    [HttpPut("{id:guid}/images")]
    public async Task<IActionResult> Upload(
        Guid id,
        [FromForm(Name = "Data")] IFormFileCollection files)
    {
        await _storageService.UploadFiles(id, [.. files], FileSource.Product);

        return Ok();
    }

    [HttpGet("{id:guid}/images-links")]
    public async Task<IActionResult> ListLinks(Guid id)
    {
        var result = await _storageService.ListFilesLinks(id, FileSource.Product);

        var fileResults = result.Select(image =>
        {
            if (image is not null)
            {
                return new ProductImagesLinks
                {
                    Key = image.Key,
                    DownloadLink = image.DownloadLink
                };
            }

            return null;

        }).ToList();

        return Ok(fileResults);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(List<string> keys)
    {
        await _storageService.DeleteFiles(keys, FileSource.Product);

        return Ok();
    }
}
