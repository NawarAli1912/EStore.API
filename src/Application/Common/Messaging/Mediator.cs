using System.Reflection;
using System.Runtime.ExceptionServices;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Messaging;

namespace Application.Common.Messaging;

/// <summary>
/// Minimal in-house mediator that replaces MediatR. Resolves the single
/// request handler and the matching pipeline behaviors from the container and
/// runs them in registration order (first registered = outermost).
/// </summary>
public sealed class Mediator(IServiceProvider serviceProvider) : ISender, IPublisher
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public Task<TResponse> Send<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestType = request.GetType();

        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
        var handler = _serviceProvider.GetService(handlerType)
            ?? throw new InvalidOperationException(
                $"No handler registered for request '{requestType.Name}'.");
        var handleMethod = handlerType.GetMethod("Handle")!;

        RequestHandlerDelegate<TResponse> pipeline = () =>
            (Task<TResponse>)Invoke(handleMethod, handler, [request, cancellationToken]);

        var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, typeof(TResponse));
        var behaviorHandle = behaviorType.GetMethod("Handle")!;
        var behaviors = _serviceProvider.GetServices(behaviorType).Where(b => b is not null).ToList();

        for (var i = behaviors.Count - 1; i >= 0; i--)
        {
            var behavior = behaviors[i]!;
            var next = pipeline;
            pipeline = () =>
                (Task<TResponse>)Invoke(behaviorHandle, behavior, [request, next, cancellationToken]);
        }

        return pipeline();
    }

    public Task Publish(object notification, CancellationToken cancellationToken = default)
        => PublishCore(notification, cancellationToken);

    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
        => PublishCore(notification!, cancellationToken);

    private async Task PublishCore(object notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        var handlerType = typeof(INotificationHandler<>).MakeGenericType(notification.GetType());
        var handleMethod = handlerType.GetMethod("Handle")!;

        foreach (var handler in _serviceProvider.GetServices(handlerType))
        {
            if (handler is null)
            {
                continue;
            }

            await (Task)Invoke(handleMethod, handler, [notification, cancellationToken]);
        }
    }

    private static object Invoke(MethodInfo method, object target, object?[] arguments)
    {
        try
        {
            return method.Invoke(target, arguments)!;
        }
        catch (TargetInvocationException ex) when (ex.InnerException is not null)
        {
            ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            throw;
        }
    }
}
