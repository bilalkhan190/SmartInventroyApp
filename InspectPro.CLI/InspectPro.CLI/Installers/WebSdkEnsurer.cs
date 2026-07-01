using CaBootstrap.CLI.Models;
using CaBootstrap.CLI.Utilities;
using Microsoft.Extensions.Logging;

namespace CaBootstrap.CLI.Installers;

public sealed class WebSdkEnsurer
{
    private readonly ILogger<WebSdkEnsurer> _logger;

    public WebSdkEnsurer(ILogger<WebSdkEnsurer> logger)
    {
        _logger = logger;
    }

    public void EnsureApiProjectsUseWebSdk(IReadOnlyList<ProjectInfo> projects)
    {
        foreach (var project in projects.Where(p => p.Role == ProjectRole.Api))
        {
            if (ProjectFileReader.UsesWebSdk(project.FilePath))
            {
                continue;
            }

            var lines = File.ReadAllLines(project.FilePath).ToList();
            var sdkLineIndex = lines.FindIndex(line =>
                line.Contains("<Project Sdk=", StringComparison.OrdinalIgnoreCase));

            if (sdkLineIndex < 0)
            {
                continue;
            }

            lines[sdkLineIndex] = lines[sdkLineIndex].Replace(
                "Microsoft.NET.Sdk\"",
                "Microsoft.NET.Sdk.Web\"",
                StringComparison.OrdinalIgnoreCase);

            if (lines[sdkLineIndex].Contains("Microsoft.NET.Sdk.Web", StringComparison.OrdinalIgnoreCase))
            {
                File.WriteAllLines(project.FilePath, lines);
                _logger.LogInformation(
                    "Updated {Project} to use Microsoft.NET.Sdk.Web for ASP.NET packages",
                    project.Name);
            }
        }
    }
}
