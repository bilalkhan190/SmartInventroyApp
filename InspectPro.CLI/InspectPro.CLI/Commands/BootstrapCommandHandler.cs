using CaBootstrap.CLI.Abstractions;
using Microsoft.Extensions.Logging;

namespace CaBootstrap.CLI.Commands;

public sealed class BootstrapCommandHandler
{
    private readonly IBootstrapService _bootstrapService;
    private readonly ILogger<BootstrapCommandHandler> _logger;

    public BootstrapCommandHandler(
        IBootstrapService bootstrapService,
        ILogger<BootstrapCommandHandler> logger)
    {
        _bootstrapService = bootstrapService;
        _logger = logger;
    }

    public async Task<int> HandleAsync(
        string? path,
        string? config,
        CancellationToken cancellationToken)
    {
        var workingDirectory = string.IsNullOrWhiteSpace(path)
            ? Directory.GetCurrentDirectory()
            : Path.GetFullPath(path);

        _logger.LogInformation("ca-bootstrap starting in {Directory}", workingDirectory);

        var result = await _bootstrapService.BootstrapAsync(
            workingDirectory,
            config,
            cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                _logger.LogError("{Error}", error);
            }

            return 1;
        }

        foreach (var warning in result.Warnings)
        {
            _logger.LogWarning("{Warning}", warning);
        }

        _logger.LogInformation("ca-bootstrap completed successfully");
        return 0;
    }
}
