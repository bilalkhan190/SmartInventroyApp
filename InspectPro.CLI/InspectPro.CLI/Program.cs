using CaBootstrap.CLI.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CaBootstrap.CLI;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddCaBootstrapServices();
                services.AddTransient<BootstrapCommandHandler>();
            })
            .Build();

        var rootCommand = BootstrapRootCommand.Create(host.Services);
        return await rootCommand.Parse(args).InvokeAsync();
    }
}
