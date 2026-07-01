using CaBootstrap.CLI.Abstractions;
using CaBootstrap.CLI.Installers;
using CaBootstrap.CLI.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace CaBootstrap.CLI.Tests;

public class PackageInstallerTests
{
    [Fact]
    public async Task InstallAsync_ContinuesOnFailureAndReturnsErrors()
    {
        var runner = new Mock<IDotNetCommandRunner>();
        runner
            .Setup(r => r.AddPackageAsync(It.IsAny<string>(), "BadPackage", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DotNetCommandResult
            {
                ExitCode = 1,
                Command = "dotnet add package BadPackage",
                StandardError = "Package not found",
                StandardOutput = string.Empty
            });
        runner
            .Setup(r => r.AddPackageAsync(It.IsAny<string>(), "MediatR", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DotNetCommandResult
            {
                ExitCode = 0,
                Command = "dotnet add package MediatR",
                StandardError = string.Empty,
                StandardOutput = string.Empty
            });

        var installer = new PackageInstaller(runner.Object, NullLogger<PackageInstaller>.Instance);
        var projects = new List<ProjectInfo>
        {
            new()
            {
                Name = "App.Application",
                Role = ProjectRole.Application,
                DirectoryPath = "C:\\app",
                FilePath = "C:\\app\\App.Application.csproj"
            }
        };

        var config = new BootstrapConfiguration
        {
            Packages = new Dictionary<string, List<string>>
            {
                ["Application"] = ["BadPackage", "MediatR"]
            }
        };

        var errors = await installer.InstallAsync(projects, config);

        Assert.Single(errors);
        Assert.Contains("BadPackage", errors[0]);
    }
}
