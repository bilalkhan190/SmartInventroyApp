using CaBootstrap.CLI.Abstractions;
using CaBootstrap.CLI.Installers;
using CaBootstrap.CLI.Services;
using CaBootstrap.CLI.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CaBootstrap.CLI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCaBootstrapServices(this IServiceCollection services)
    {
        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddProvider(new ProgressConsoleLoggerProvider());
            builder.SetMinimumLevel(LogLevel.Information);
        });

        services.AddSingleton<IConfigurationService, ConfigurationService>();
        services.AddSingleton<IProjectScanner, ProjectScanner>();
        services.AddSingleton<IDotNetCommandRunner, DotNetCommandRunner>();
        services.AddSingleton<IPackageInstaller, PackageInstaller>();
        services.AddSingleton<WebSdkEnsurer>();
        services.AddSingleton<IReferenceInstaller, ReferenceInstaller>();
        services.AddSingleton<IFolderGenerator, FolderGenerator>();
        services.AddSingleton<ITemplateGenerator, TemplateGenerator>();
        services.AddSingleton<IBootstrapService, BootstrapService>();

        return services;
    }
}
