using Microsoft.AspNetCore.Mvc;
using Nest;
using Presentation.Controllers.Common;

namespace Presentation.Controllers;

[Route("api/elastic")]
public sealed class ElasticController(IElasticClient client) : ApiController
{
    private readonly IElasticClient _client = client;

    [HttpGet("ping")]
    public async Task<IActionResult> CheckHealth()
    {
        var result = await _client.PingAsync();

        return Ok(result.IsValid);
    }
}
