using CaBootstrap.CLI.Abstractions;
using CaBootstrap.CLI.Models;
using Microsoft.Extensions.Logging;

namespace CaBootstrap.CLI.Installers;

public sealed class ReferenceInstaller : IReferenceInstaller
{
    private readonly IDotNetCommandRunner _commandRunner;
    private readonly ILogger<ReferenceInstaller> _logger;

    public ReferenceInstaller(IDotNetCommandRunner commandRunner, ILogger<ReferenceInstaller> logger)
    {
        _commandRunner = commandRunner;
        _logger = logger;
    }

    public async Task<IReadOnlyList<string>> InstallAsync(
        IReadOnlyList<ProjectInfo> projects,
        BootstrapConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        var errors = new List<string>();
        var byRole = projects
            .Where(p => p.Role != ProjectRole.Unknown)
            .GroupBy(p => p.Role)
            .ToDictionary(g => g.Key, g => g.First());

        foreach (var rule in configuration.References)
        {
            if (!Enum.TryParse<ProjectRole>(rule.From, ignoreCase: true, out var fromRole) ||
                !Enum.TryParse<ProjectRole>(rule.To, ignoreCase: true, out var toRole))
            {
                errors.Add($"Invalid reference rule: {rule.From} -> {rule.To}");
                continue;
            }

            if (!byRole.TryGetValue(fromRole, out var fromProject))
            {
                errors.Add($"Reference source project not found for role: {rule.From}");
                continue;
            }

            if (!byRole.TryGetValue(toRole, out var toProject))
            {
                errors.Add($"Reference target project not found for role: {rule.To}");
                continue;
            }

            if (ReferenceExists(fromProject.FilePath, toProject.FilePath))
            {
                _logger.LogInformation(
                    "Reference already exists: {From} -> {To}",
                    fromProject.Name,
                    toProject.Name);
                continue;
            }

            _logger.LogInformation(
                "Adding reference: {From} -> {To}",
                fromProject.Name,
                toProject.Name);

            var result = await _commandRunner.AddReferenceAsync(
                fromProject.FilePath,
                toProject.FilePath,
                cancellationToken);

            if (!result.Succeeded)
            {
                var message =
                    $"Failed to add reference {fromProject.Name} -> {toProject.Name}: {result.StandardError}".Trim();
                _logger.LogError("{Message}", message);
                errors.Add(message);
            }
        }

        return errors;
    }

    internal static bool ReferenceExists(string fromProjectPath, string toProjectPath)
    {
        var content = File.ReadAllText(fromProjectPath);
        var referenceName = Path.GetFileName(toProjectPath);
        return content.Contains(referenceName, StringComparison.OrdinalIgnoreCase);
    }
}
