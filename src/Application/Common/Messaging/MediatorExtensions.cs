using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Messaging;

namespace Application.Common.Messaging;

public static class MediatorExtensions
{
    /// <summary>
    /// Registers the mediator plus every IRequestHandler&lt;,&gt; and
    /// INotificationHandler&lt;&gt; implementation found in the given assemblies.
    /// Pipeline behaviors are registered separately by the caller, in the order
    /// they should execute.
    /// </summary>
    public static IServiceCollection AddMediator(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddScoped<Mediator>();
        services.AddScoped<ISender>(sp => sp.GetRequiredService<Mediator>());
        services.AddScoped<IPublisher>(sp => sp.GetRequiredService<Mediator>());

        foreach (var assembly in assemblies)
        {
            var implementations = assembly.GetTypes()
                .Where(type => type is { IsAbstract: false, IsInterface: false });

            foreach (var implementation in implementations)
            {
                var handlerInterfaces = implementation.GetInterfaces()
                    .Where(i => i.IsGenericType &&
                        (i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
                         i.GetGenericTypeDefinition() == typeof(INotificationHandler<>)));

                foreach (var handlerInterface in handlerInterfaces)
                {
                    services.AddScoped(handlerInterface, implementation);
                }
            }
        }

        return services;
    }
}
