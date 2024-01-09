using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Common;

[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorsController : ControllerBase
{
    [Route("/error")]
    public IActionResult Error()
    {
        var exception = HttpContext
            .Features
            .Get<IExceptionHandlerFeature>()?.Error;

        return Problem(title: "Server error.", detail: exception!.Message);
    }
}
