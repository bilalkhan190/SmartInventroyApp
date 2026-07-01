using CaBootstrap.CLI.Abstractions;
using CaBootstrap.CLI.Installers;
using CaBootstrap.CLI.Models;
using Microsoft.Extensions.Logging;

namespace CaBootstrap.CLI.Services;

public sealed class BootstrapService : IBootstrapService
{
    private readonly IConfigurationService _configurationService;
    private readonly IProjectScanner _projectScanner;
    private readonly IPackageInstaller _packageInstaller;
    private readonly IReferenceInstaller _referenceInstaller;
    private readonly IFolderGenerator _folderGenerator;
    private readonly ITemplateGenerator _templateGenerator;
    private readonly IDotNetCommandRunner _commandRunner;
    private readonly WebSdkEnsurer _webSdkEnsurer;
    private readonly ILogger<BootstrapService> _logger;

    public BootstrapService(
        IConfigurationService configurationService,
        IProjectScanner projectScanner,
        IPackageInstaller packageInstaller,
        IReferenceInstaller referenceInstaller,
        IFolderGenerator folderGenerator,
        ITemplateGenerator templateGenerator,
        IDotNetCommandRunner commandRunner,
        WebSdkEnsurer webSdkEnsurer,
        ILogger<BootstrapService> logger)
    {
        _configurationService = configurationService;
        _projectScanner = projectScanner;
        _packageInstaller = packageInstaller;
        _referenceInstaller = referenceInstaller;
        _folderGenerator = folderGenerator;
        _templateGenerator = templateGenerator;
        _commandRunner = commandRunner;
        _webSdkEnsurer = webSdkEnsurer;
        _logger = logger;
    }

    public async Task<BootstrapResult> BootstrapAsync(
        string rootDirectory,
        string? configFilePath = null,
        CancellationToken cancellationToken = default)
    {
        var errors = new List<string>();
        var warnings = new List<string>();

        rootDirectory = Path.GetFullPath(rootDirectory);
        if (!Directory.Exists(rootDirectory))
        {
            return BootstrapResult.Failure($"Directory not found: {rootDirectory}");
        }

        var configuration = _configurationService.Load(rootDirectory, configFilePath);
        var projects = _projectScanner.Scan(rootDirectory, configuration);

        var validationErrors = ValidateStructure(projects);
        if (validationErrors.Count > 0)
        {
            warnings.AddRange(validationErrors);
            foreach (var warning in validationErrors)
            {
                _logger.LogWarning("{Warning}", warning);
            }
        }

        if (projects.All(p => p.Role == ProjectRole.Unknown))
        {
            return BootstrapResult.Failure("No recognizable Clean Architecture projects were found.");
        }

        if (configuration.Features.AddReferences)
        {
            _logger.LogInformation("Adding references...");
            errors.AddRange(await _referenceInstaller.InstallAsync(projects, configuration, cancellationToken));
        }

        if (configuration.Features.InstallPackages)
        {
            _webSdkEnsurer.EnsureApiProjectsUseWebSdk(projects);
            errors.AddRange(await _packageInstaller.InstallAsync(projects, configuration, cancellationToken));
        }

        if (configuration.Features.CreateFolders)
        {
            _logger.LogInformation("Creating folders...");
            _folderGenerator.Generate(projects, configuration);
        }

        if (configuration.Features.GenerateTemplates)
        {
            _logger.LogInformation("Generating template files...");
            var configDir = ResolveConfigurationDirectory(rootDirectory, configFilePath);
            _templateGenerator.Generate(projects, configuration, configDir);
        }

        if (configuration.Features.Restore)
        {
            _logger.LogInformation("Restoring packages...");
            var restore = await _commandRunner.RestoreAsync(rootDirectory, cancellationToken);
            if (!restore.Succeeded)
            {
                errors.Add($"Restore failed: {restore.StandardError}".Trim());
            }
        }

        if (configuration.Features.Build)
        {
            _logger.LogInformation("Building solution...");
            var build = await _commandRunner.BuildAsync(rootDirectory, cancellationToken);
            if (build.Succeeded)
            {
                _logger.LogInformation("Build succeeded");
            }
            else
            {
                errors.Add($"Build failed: {build.StandardError}".Trim());
            }
        }

        if (configuration.Features.RunTests)
        {
            _logger.LogInformation("Running tests...");
            var test = await _commandRunner.TestAsync(rootDirectory, cancellationToken);
            if (!test.Succeeded)
            {
                errors.Add($"Tests failed: {test.StandardError}".Trim());
            }
        }

        if (errors.Count > 0)
        {
            return new BootstrapResult
            {
                Succeeded = false,
                Errors = errors,
                Warnings = warnings
            };
        }

        return BootstrapResult.Success(warnings);
    }

    internal static IReadOnlyList<string> ValidateStructure(IReadOnlyList<ProjectInfo> projects)
    {
        var warnings = new List<string>();
        var requiredRoles = new[] { ProjectRole.Application, ProjectRole.Domain };

        foreach (var role in requiredRoles)
        {
            if (projects.All(p => p.Role != role))
            {
                warnings.Add($"Recommended project role not found: {role}");
            }
        }

        return warnings;
    }

    private static string ResolveConfigurationDirectory(string rootDirectory, string? configFilePath)
    {
        if (string.IsNullOrWhiteSpace(configFilePath))
        {
            return rootDirectory;
        }

        return Path.IsPathRooted(configFilePath)
            ? Path.GetDirectoryName(configFilePath)!
            : rootDirectory;
    }
}
