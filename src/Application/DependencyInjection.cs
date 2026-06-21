using System.Reflection;
using Application.Common.Behaviors;
using Application.Common.Messaging;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Messaging;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediator(typeof(DependencyInjection).Assembly);

        // Registration order is execution order: first registered runs outermost.
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(QueryCachingPipelineBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(IdempotentPipelineBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
