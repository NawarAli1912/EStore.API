using Mapster;
using MapsterMapper;
using System.Reflection;

namespace Presentation.Common.Mapping;

public static class DependencyInjection
{
    public static IServiceCollection AddMapping(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;

        TypeAdapterConfig.GlobalSettings.Default
            .AddDestinationTransform(DestinationTransform.EmptyCollectionIfNull);

        config.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton(config);

        services.AddScoped<IMapper, ServiceMapper>();

        return services;
    }
}
