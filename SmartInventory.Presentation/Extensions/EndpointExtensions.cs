using System.Reflection;
using SmartInventory.Presentation.Endpoints;

namespace SmartInventory.Presentation.Extensions;

public static class EndpointExtensions
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly? assembly = null)
    {
        assembly ??= typeof(IEndpoint).Assembly;

        var endpointTypes = assembly
            .GetTypes()
            .Where(type => typeof(IEndpoint).IsAssignableFrom(type)
                && type is { IsAbstract: false, IsInterface: false });

        foreach (var type in endpointTypes)
        {
            services.AddSingleton(typeof(IEndpoint), type);
        }

        return services;
    }

    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.ServiceProvider.GetServices<IEndpoint>();

        foreach (var endpoint in endpoints)
        {
            endpoint.MapEndpoint(app);
        }

        return app;
    }
}
