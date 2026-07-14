using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SmartInventory.Application.Behaviors;
using SmartInventory.Application.Common.DomainEvents;
using SmartInventory.Application.Contracts;
using System.Reflection;

namespace SmartInventory.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        return services;
    }
}
