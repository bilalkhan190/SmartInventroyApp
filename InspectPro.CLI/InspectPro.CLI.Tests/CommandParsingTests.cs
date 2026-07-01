using CaBootstrap.CLI.Abstractions;
using CaBootstrap.CLI.Commands;
using CaBootstrap.CLI.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace CaBootstrap.CLI.Tests;

public class CommandParsingTests
{
    [Fact]
    public void RootCommand_ContainsInitAndSetupCommands()
    {
        var services = new ServiceCollection()
            .AddSingleton(NullLogger<BootstrapCommandHandler>.Instance)
            .AddSingleton<IBootstrapService, NoOpBootstrapService>()
            .AddTransient<BootstrapCommandHandler>()
            .BuildServiceProvider();

        var root = BootstrapRootCommand.Create(services);

        Assert.Equal(2, root.Subcommands.Count);
        Assert.Contains(root.Subcommands, c => c.Name == "init");
        Assert.Contains(root.Subcommands, c => c.Name == "setup");
    }

    [Fact]
    public async Task BootstrapCommandHandler_ReturnsZeroWhenBootstrapSucceeds()
    {
        var service = new TrackingBootstrapService();
        var handler = new BootstrapCommandHandler(service, NullLogger<BootstrapCommandHandler>.Instance);

        var exitCode = await handler.HandleAsync(null, null, CancellationToken.None);

        Assert.Equal(0, exitCode);
        Assert.True(service.WasCalled);
    }

    [Fact]
    public async Task BootstrapCommandHandler_ReturnsOneWhenBootstrapFails()
    {
        var handler = new BootstrapCommandHandler(
            new FailingBootstrapService(),
            NullLogger<BootstrapCommandHandler>.Instance);

        var exitCode = await handler.HandleAsync(null, null, CancellationToken.None);

        Assert.Equal(1, exitCode);
    }

    private sealed class TrackingBootstrapService : IBootstrapService
    {
        public bool WasCalled { get; private set; }

        public Task<BootstrapResult> BootstrapAsync(
            string rootDirectory,
            string? configFilePath = null,
            CancellationToken cancellationToken = default)
        {
            WasCalled = true;
            return Task.FromResult(BootstrapResult.Success());
        }
    }

    private sealed class FailingBootstrapService : IBootstrapService
    {
        public Task<BootstrapResult> BootstrapAsync(
            string rootDirectory,
            string? configFilePath = null,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(BootstrapResult.Failure("bootstrap failed"));
    }

    private sealed class NoOpBootstrapService : IBootstrapService
    {
        public Task<BootstrapResult> BootstrapAsync(
            string rootDirectory,
            string? configFilePath = null,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(BootstrapResult.Success());
    }
}
