using CaBootstrap.CLI.Abstractions;
using CaBootstrap.CLI.Models;
using CaBootstrap.CLI.Utilities;
using Microsoft.Extensions.Logging;

namespace CaBootstrap.CLI.Installers;

public sealed class FolderGenerator : IFolderGenerator
{
    private readonly ILogger<FolderGenerator> _logger;

    public FolderGenerator(ILogger<FolderGenerator> logger)
    {
        _logger = logger;
    }

    public IReadOnlyList<string> Generate(
        IReadOnlyList<ProjectInfo> projects,
        BootstrapConfiguration configuration)
    {
        var created = new List<string>();

        foreach (var project in projects.Where(p => p.Role != ProjectRole.Unknown))
        {
            if (!configuration.Folders.TryGetValue(project.RoleKey, out var folders))
            {
                continue;
            }

            foreach (var folder in folders)
            {
                var path = Path.Combine(project.DirectoryPath, folder);
                var isNewFolder = !Directory.Exists(path);

                if (isNewFolder)
                {
                    Directory.CreateDirectory(path);
                    created.Add(path);
                    _logger.LogInformation("Created folder: {Path}", path);
                }

                if (ProjectFolderRegistrar.RegisterFolder(project.FilePath, folder))
                {
                    _logger.LogInformation("Registered folder in project: {Project}/{Folder}", project.Name, folder);
                }
            }
        }

        if (created.Count > 0)
        {
            _logger.LogInformation("Creating folders completed ({Count} created)", created.Count);
        }
        else
        {
            _logger.LogInformation("All configured folders already exist");
        }

        return created;
    }
}
