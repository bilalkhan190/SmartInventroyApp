using CaBootstrap.CLI.Abstractions;
using CaBootstrap.CLI.Models;
using CaBootstrap.CLI.Utilities;
using Microsoft.Extensions.Logging;

namespace CaBootstrap.CLI.Services;

public sealed class ProjectScanner : IProjectScanner
{
    private readonly ILogger<ProjectScanner> _logger;

    public ProjectScanner(ILogger<ProjectScanner> logger)
    {
        _logger = logger;
    }

    public IReadOnlyList<ProjectInfo> Scan(string rootDirectory, BootstrapConfiguration configuration)
    {
        if (!Directory.Exists(rootDirectory))
        {
            throw new DirectoryNotFoundException($"Directory not found: {rootDirectory}");
        }

        var projectFiles = Directory
            .EnumerateFiles(rootDirectory, "*.csproj", SearchOption.AllDirectories)
            .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
            .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (projectFiles.Count == 0)
        {
            _logger.LogWarning("No .csproj files found in {Directory}", rootDirectory);
            return [];
        }

        var projects = projectFiles
            .Select(path =>
            {
                var name = Path.GetFileNameWithoutExtension(path);
                var role = ProjectRoleDetector.ResolveRole(name, configuration.ProjectNames);
                return new ProjectInfo
                {
                    Name = name,
                    FilePath = Path.GetFullPath(path),
                    DirectoryPath = Path.GetDirectoryName(path)!,
                    Role = role
                };
            })
            .ToList();

        ApplyConfiguredNameOverrides(projects, configuration.ProjectNames);

        foreach (var project in projects.Where(p => p.Role != ProjectRole.Unknown))
        {
            _logger.LogInformation("Found {Role} project: {Name}", project.Role, project.Name);
        }

        var unknown = projects.Where(p => p.Role == ProjectRole.Unknown).ToList();
        foreach (var project in unknown)
        {
            _logger.LogWarning("Could not determine role for project: {Name}", project.Name);
        }

        return projects;
    }

    private static void ApplyConfiguredNameOverrides(
        List<ProjectInfo> projects,
        IReadOnlyDictionary<string, string> configuredNames)
    {
        foreach (var (roleKey, projectName) in configuredNames)
        {
            if (string.IsNullOrWhiteSpace(projectName))
            {
                continue;
            }

            if (!Enum.TryParse<ProjectRole>(roleKey, ignoreCase: true, out var role))
            {
                continue;
            }

            var match = projects.FirstOrDefault(p =>
                p.Name.Equals(projectName, StringComparison.OrdinalIgnoreCase));

            if (match is null)
            {
                continue;
            }

            var index = projects.IndexOf(match);
            projects[index] = new ProjectInfo
            {
                Name = match.Name,
                FilePath = match.FilePath,
                DirectoryPath = match.DirectoryPath,
                Role = role
            };
        }
    }
}
