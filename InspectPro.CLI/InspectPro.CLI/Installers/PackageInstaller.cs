using CaBootstrap.CLI.Abstractions;
using CaBootstrap.CLI.Models;
using CaBootstrap.CLI.Utilities;
using Microsoft.Extensions.Logging;

namespace CaBootstrap.CLI.Installers;

public sealed class PackageInstaller : IPackageInstaller
{
    private readonly IDotNetCommandRunner _commandRunner;
    private readonly ILogger<PackageInstaller> _logger;

    public PackageInstaller(IDotNetCommandRunner commandRunner, ILogger<PackageInstaller> logger)
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

        foreach (var project in projects.Where(p => p.Role != ProjectRole.Unknown))
        {
            if (!configuration.Packages.TryGetValue(project.RoleKey, out var packages) || packages.Count == 0)
            {
                continue;
            }

            var targetFramework = ProjectFileReader.GetTargetFramework(project.FilePath);

            foreach (var package in packages)
            {
                if (ProjectFileReader.PackageReferenceExists(project.FilePath, package))
                {
                    _logger.LogInformation(
                        "Package {Package} already exists in {Project}, skipping",
                        package,
                        project.Name);
                    continue;
                }

                var version = PackageVersionResolver.Resolve(package, targetFramework);
                _logger.LogInformation(
                    "Installing {Package}{Version} in {Project}...",
                    package,
                    version is null ? string.Empty : $" ({version})",
                    project.Name);

                var result = await _commandRunner.AddPackageAsync(
                    project.FilePath,
                    package,
                    version,
                    cancellationToken);

                if (!result.Succeeded)
                {
                    var details = GetFailureDetails(result);
                    var message = $"Failed to install {package} in {project.Name}: {details}".Trim();
                    _logger.LogError("{Message}", message);
                    errors.Add(message);
                }
            }
        }

        return errors;
    }

    internal static string GetFailureDetails(Models.DotNetCommandResult result)
    {
        if (!string.IsNullOrWhiteSpace(result.StandardError))
        {
            return result.StandardError.Trim();
        }

        var errorLines = result.StandardOutput
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .Where(line => line.StartsWith("error:", StringComparison.OrdinalIgnoreCase))
            .ToList();

        return errorLines.Count > 0
            ? string.Join(Environment.NewLine, errorLines)
            : result.StandardOutput.Trim();
    }
}
