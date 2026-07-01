using CaBootstrap.CLI.Installers;
using CaBootstrap.CLI.Models;
using Microsoft.Extensions.Logging.Abstractions;

namespace CaBootstrap.CLI.Tests;

public class FolderGeneratorTests : IDisposable
{
    private readonly string _root;

    public FolderGeneratorTests()
    {
        _root = Path.Combine(Path.GetTempPath(), "inspectpro-folder-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_root);
    }

    public void Dispose()
    {
        if (Directory.Exists(_root))
        {
            Directory.Delete(_root, recursive: true);
        }
    }

    [Fact]
    public void Generate_CreatesConfiguredFolders()
    {
        var projectDir = Path.Combine(_root, "App.Application");
        Directory.CreateDirectory(projectDir);

        var generator = new FolderGenerator(NullLogger<FolderGenerator>.Instance);
        var projects = new List<ProjectInfo>
        {
            new()
            {
                Name = "App.Application",
                Role = ProjectRole.Application,
                DirectoryPath = projectDir,
                FilePath = Path.Combine(projectDir, "App.Application.csproj")
            }
        };

        var config = new BootstrapConfiguration
        {
            Folders = new Dictionary<string, List<string>>
            {
                ["Application"] = ["Features", "Behaviors", "Common"]
            }
        };

        var created = generator.Generate(projects, config);

        Assert.Equal(3, created.Count);
        Assert.True(Directory.Exists(Path.Combine(projectDir, "Features")));
        Assert.True(Directory.Exists(Path.Combine(projectDir, "Behaviors")));
        Assert.True(Directory.Exists(Path.Combine(projectDir, "Common")));
    }

    [Fact]
    public void Generate_SkipsExistingFolders()
    {
        var projectDir = Path.Combine(_root, "App.Domain");
        var featuresDir = Path.Combine(projectDir, "Entities");
        Directory.CreateDirectory(featuresDir);

        var generator = new FolderGenerator(NullLogger<FolderGenerator>.Instance);
        var projects = new List<ProjectInfo>
        {
            new()
            {
                Name = "App.Domain",
                Role = ProjectRole.Domain,
                DirectoryPath = projectDir,
                FilePath = Path.Combine(projectDir, "App.Domain.csproj")
            }
        };

        var config = new BootstrapConfiguration
        {
            Folders = new Dictionary<string, List<string>>
            {
                ["Domain"] = ["Entities", "Enums"]
            }
        };

        var created = generator.Generate(projects, config);

        Assert.Single(created);
        Assert.True(Directory.Exists(Path.Combine(projectDir, "Enums")));
    }
}
