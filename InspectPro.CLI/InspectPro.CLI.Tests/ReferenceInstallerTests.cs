using CaBootstrap.CLI.Abstractions;
using CaBootstrap.CLI.Installers;
using CaBootstrap.CLI.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace CaBootstrap.CLI.Tests;

public class ReferenceInstallerTests : IDisposable
{
    private readonly string _root;

    public ReferenceInstallerTests()
    {
        _root = Path.Combine(Path.GetTempPath(), "inspectpro-ref-tests", Guid.NewGuid().ToString("N"));
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
    public void ReferenceExists_DetectsExistingReference()
    {
        var from = Path.Combine(_root, "Api.csproj");
        var to = Path.Combine(_root, "Application.csproj");
        File.WriteAllText(from, "<Project><ItemGroup><ProjectReference Include=\"Application.csproj\" /></ItemGroup></Project>");
        File.WriteAllText(to, "<Project></Project>");

        Assert.True(ReferenceInstaller.ReferenceExists(from, to));
    }

    [Fact]
    public async Task InstallAsync_AddsMissingReferences()
    {
        var apiPath = Path.Combine(_root, "Api.csproj");
        var appPath = Path.Combine(_root, "Application.csproj");
        File.WriteAllText(apiPath, "<Project></Project>");
        File.WriteAllText(appPath, "<Project></Project>");

        var runner = new Mock<IDotNetCommandRunner>();
        runner
            .Setup(r => r.AddReferenceAsync(apiPath, appPath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DotNetCommandResult
            {
                ExitCode = 0,
                Command = "dotnet add reference",
                StandardError = string.Empty,
                StandardOutput = string.Empty
            });

        var installer = new ReferenceInstaller(runner.Object, NullLogger<ReferenceInstaller>.Instance);
        var projects = new List<ProjectInfo>
        {
            new() { Name = "Api", Role = ProjectRole.Api, DirectoryPath = _root, FilePath = apiPath },
            new() { Name = "Application", Role = ProjectRole.Application, DirectoryPath = _root, FilePath = appPath }
        };

        var config = new BootstrapConfiguration
        {
            References =
            [
                new ReferenceRule { From = "Api", To = "Application" }
            ]
        };

        var errors = await installer.InstallAsync(projects, config);

        Assert.Empty(errors);
        runner.Verify(r => r.AddReferenceAsync(apiPath, appPath, It.IsAny<CancellationToken>()), Times.Once);
    }
}
