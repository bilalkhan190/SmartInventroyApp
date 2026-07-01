using CaBootstrap.CLI.Models;
using CaBootstrap.CLI.Services;
using CaBootstrap.CLI.Utilities;
using Microsoft.Extensions.Logging.Abstractions;

namespace CaBootstrap.CLI.Tests;

public class ProjectScannerTests : IDisposable
{
    private readonly string _root;

    public ProjectScannerTests()
    {
        _root = Path.Combine(Path.GetTempPath(), "inspectpro-tests", Guid.NewGuid().ToString("N"));
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
    public void Scan_DiscoversProjectsAndAssignsRoles()
    {
        CreateProject("SmartInventory.Domain");
        CreateProject("SmartInventory.Application");
        CreateProject("SmartInventory.Infrastructure");
        CreateProject("SmartInventory.Presentation");

        var scanner = new ProjectScanner(NullLogger<ProjectScanner>.Instance);
        var config = new BootstrapConfiguration();
        var projects = scanner.Scan(_root, config);

        Assert.Equal(4, projects.Count);
        Assert.Contains(projects, p => p.Role == ProjectRole.Domain);
        Assert.Contains(projects, p => p.Role == ProjectRole.Application);
        Assert.Contains(projects, p => p.Role == ProjectRole.Infrastructure);
        Assert.Contains(projects, p => p.Role == ProjectRole.Api);
    }

    [Fact]
    public void Scan_UsesConfiguredProjectNameOverride()
    {
        CreateProject("MyCompany.Presentation");

        var scanner = new ProjectScanner(NullLogger<ProjectScanner>.Instance);
        var config = new BootstrapConfiguration
        {
            ProjectNames = new Dictionary<string, string> { ["Api"] = "MyCompany.Presentation" }
        };

        var projects = scanner.Scan(_root, config);
        var api = Assert.Single(projects);

        Assert.Equal(ProjectRole.Api, api.Role);
    }

    [Fact]
    public void ProjectRoleDetector_IdentifiesRolesByToken()
    {
        Assert.Equal(ProjectRole.Api, ProjectRoleDetector.Detect("Contoso.Api"));
        Assert.Equal(ProjectRole.Api, ProjectRoleDetector.Detect("Contoso.Web"));
        Assert.Equal(ProjectRole.Application, ProjectRoleDetector.Detect("Contoso.Application"));
        Assert.Equal(ProjectRole.Infrastructure, ProjectRoleDetector.Detect("Contoso.Infrastructure"));
        Assert.Equal(ProjectRole.Domain, ProjectRoleDetector.Detect("Contoso.Domain"));
        Assert.Equal(ProjectRole.Unknown, ProjectRoleDetector.Detect("Contoso.Shared"));
    }

    private void CreateProject(string name)
    {
        var dir = Path.Combine(_root, name);
        Directory.CreateDirectory(dir);
        File.WriteAllText(Path.Combine(dir, $"{name}.csproj"), "<Project Sdk=\"Microsoft.NET.Sdk\"></Project>");
    }
}
