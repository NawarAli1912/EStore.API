using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Presentation.Common.Http;
using SharedKernel.Primitives;

namespace Presentation.Controllers.Common;

[ApiController]
[Route("api")]
public class ApiController : ControllerBase
{
    protected IActionResult Problem(List<Error> errors)
    {
        if (errors.Count == 0)
        {
            return Problem();
        }

        if (errors.All(e => e.Type == ErrorType.Validation))
        {
            return ValidationProblem(errors);
        }

        HttpContext.Items[HttpContextItemKeys.Errors] = errors;
        if (errors.Count > 1)
        {
            var aggregatedDetails =
                string.Join("; ", errors.Select(e => e.Description));
            var aggregatedTitles =
                string.Join(", ", errors.Select(e => GetTitle(e.Type)));

            return Problem(
                title: aggregatedTitles,
                detail: aggregatedDetails,
                statusCode: StatusCodes.Status400BadRequest);
        }

        var error = errors[0];

        ProblemDetails problem = new ProblemDetails
        {

        };

        return Problem(
            title: GetTitle(error.Type),
            detail: error.Description,
            statusCode: GetStatusCode(error.Type));
    }

    private IActionResult ValidationProblem(List<Error> errors)
    {
        ModelStateDictionary modelStateDictionary = [];

        foreach (var error in errors)
        {
            modelStateDictionary.AddModelError(
                error.Code,
                error.Description);
        }

        return ValidationProblem(modelStateDictionary);
    }

    private int GetStatusCode(ErrorType type) =>
        type switch
        {
            ErrorType.Failure => StatusCodes.Status400BadRequest,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

    private string GetTitle(ErrorType type) =>
        type switch
        {
            ErrorType.Failure => "Bad Request",
            ErrorType.Validation => "Bad Request",
            ErrorType.NotFound => "Not Found",
            ErrorType.Conflict => "Conflict",
            _ => "Server Failure"
        };

}
