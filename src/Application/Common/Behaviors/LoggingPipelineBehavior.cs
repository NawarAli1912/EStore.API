using MediatR;
using Serilog;
using System.Diagnostics;

namespace Application.Common.Behaviors;
public sealed class LoggingPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var stopWatch = new Stopwatch();
        TResponse result;
        try
        {
            stopWatch.Start();
            Log.Information($"Start Execution {typeof(TRequest).Name}.");

            result = await next();

            stopWatch.Stop();
            Log.Information($"Execution done {typeof(TRequest).Name}.; ElapsedTime: {stopWatch.ElapsedMilliseconds}ms");

        }
        catch (Exception ex)
        {
            stopWatch.Stop();
            Log.Error(ex, $"Error executing {typeof(TRequest).Name}; ElapsedTime: {stopWatch.ElapsedMilliseconds}ms");
            throw;
        }

        return result;
    }
}
