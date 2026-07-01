using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;

namespace CaBootstrap.CLI.Commands;

public static class BootstrapRootCommand
{
    public static RootCommand Create(IServiceProvider services)
    {
        var root = new RootCommand("ca-bootstrap — Clean Architecture .NET solution bootstrap tool")
        {
            CreateBootstrapCommand(services, "init", "Initialize Clean Architecture project structure."),
            CreateBootstrapCommand(services, "setup", "Set up packages, references, folders, and templates.")
        };

        return root;
    }

    private static Command CreateBootstrapCommand(
        IServiceProvider services,
        string name,
        string description)
    {
        var pathOption = new Option<DirectoryInfo?>("--path", "-p")
        {
            Description = "Root directory of the solution to bootstrap.",
            Arity = ArgumentArity.ZeroOrOne
        };

        var configOption = new Option<FileInfo?>("--config", "-c")
        {
            Description = $"Path to {BootstrapDefaults.ConfigFileName} configuration file.",
            Arity = ArgumentArity.ZeroOrOne
        };

        var command = new Command(name, description)
        {
            pathOption,
            configOption
        };

        command.SetAction(async (parseResult, cancellationToken) =>
            await ExecuteAsync(
                services,
                parseResult.GetValue(pathOption),
                parseResult.GetValue(configOption),
                cancellationToken));

        return command;
    }

    private static async Task<int> ExecuteAsync(
        IServiceProvider serviceProvider,
        DirectoryInfo? path,
        FileInfo? config,
        CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<BootstrapCommandHandler>();
        return await handler.HandleAsync(
            path?.FullName,
            config?.FullName,
            cancellationToken);
    }
}
