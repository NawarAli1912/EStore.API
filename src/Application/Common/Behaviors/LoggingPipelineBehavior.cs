using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using SharedKernel.Primitives;
using System.Diagnostics;

namespace Application.Common.Behaviors;
public sealed class LoggingPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IResult
{
    private readonly ILogger<LoggingPipelineBehavior<TRequest, TResponse>> _logger;

    public LoggingPipelineBehavior(ILogger<LoggingPipelineBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var stopWatch = new Stopwatch();
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation("Processing request {RequestName}", requestName);

        stopWatch.Start();
        TResponse result = await next();
        stopWatch.Stop();

        if (result.IsError)
        {
            using (LogContext.PushProperty("Errors", result.Errors, true))
            {
                _logger.LogError("Completed request {RequestName} with error;  ElapsedTime: {ElapsedMilliseconds}ms",
                    request,
                    stopWatch.ElapsedMilliseconds);
            }

            return result;
        }

        _logger.LogInformation("Completed request {RequestName}; {ElapsedMilliseconds}ms",
            requestName,
            stopWatch.ElapsedMilliseconds);

        return result;
    }
}
