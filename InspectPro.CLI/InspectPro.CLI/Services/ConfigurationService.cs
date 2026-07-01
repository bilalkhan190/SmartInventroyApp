using System.Reflection;
using CaBootstrap.CLI.Abstractions;
using CaBootstrap.CLI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CaBootstrap.CLI.Services;

public sealed class ConfigurationService : IConfigurationService
{
    private readonly ILogger<ConfigurationService> _logger;

    public ConfigurationService(ILogger<ConfigurationService> logger)
    {
        _logger = logger;
    }

    public BootstrapConfiguration Load(string rootDirectory, string? configFilePath = null)
    {
        var defaults = LoadDefaults();
        var userPath = ResolveConfigPath(rootDirectory, configFilePath);

        if (!File.Exists(userPath))
        {
            _logger.LogInformation(
                "Using default configuration (no {ConfigFile} found at {Path})",
                BootstrapDefaults.ConfigFileName,
                userPath);
            return defaults;
        }

        _logger.LogInformation("Loading configuration from {Path}", userPath);
        var userConfig = new ConfigurationBuilder()
            .AddJsonFile(userPath, optional: false)
            .Build()
            .Get<BootstrapConfiguration>() ?? new BootstrapConfiguration();

        return Merge(defaults, userConfig);
    }

    private static string ResolveConfigPath(string rootDirectory, string? configFilePath)
    {
        if (!string.IsNullOrWhiteSpace(configFilePath))
        {
            return Path.IsPathRooted(configFilePath)
                ? configFilePath
                : Path.Combine(rootDirectory, configFilePath);
        }

        return Path.Combine(rootDirectory, BootstrapDefaults.ConfigFileName);
    }

    private static BootstrapConfiguration LoadDefaults()
    {
        var defaultsPath = Path.Combine(AppContext.BaseDirectory, BootstrapDefaults.ConfigFileName);
        if (File.Exists(defaultsPath))
        {
            return new ConfigurationBuilder()
                .AddJsonFile(defaultsPath, optional: false)
                .Build()
                .Get<BootstrapConfiguration>() ?? new BootstrapConfiguration();
        }

        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly
            .GetManifestResourceNames()
            .FirstOrDefault(name => name.EndsWith(BootstrapDefaults.ConfigFileName, StringComparison.OrdinalIgnoreCase));

        if (resourceName is null)
        {
            return new BootstrapConfiguration();
        }

        using var stream = assembly.GetManifestResourceStream(resourceName)!;
        return new ConfigurationBuilder()
            .AddJsonStream(stream)
            .Build()
            .Get<BootstrapConfiguration>() ?? new BootstrapConfiguration();
    }

    private static BootstrapConfiguration Merge(
        BootstrapConfiguration defaults,
        BootstrapConfiguration user)
    {
        var merged = new BootstrapConfiguration
        {
            ProjectNames = new Dictionary<string, string>(defaults.ProjectNames, StringComparer.OrdinalIgnoreCase),
            Packages = CloneDictionary(defaults.Packages),
            Folders = CloneDictionary(defaults.Folders),
            References = [.. defaults.References],
            Templates = [.. defaults.Templates],
            Features = new BootstrapFeatures
            {
                InstallPackages = user.Features.InstallPackages,
                AddReferences = user.Features.AddReferences,
                CreateFolders = user.Features.CreateFolders,
                GenerateTemplates = user.Features.GenerateTemplates,
                Restore = user.Features.Restore,
                Build = user.Features.Build,
                RunTests = user.Features.RunTests
            }
        };

        foreach (var (key, value) in user.ProjectNames)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                merged.ProjectNames[key] = value;
            }
        }

        foreach (var (key, value) in user.Packages)
        {
            merged.Packages[key] = value;
        }

        foreach (var (key, value) in user.Folders)
        {
            merged.Folders[key] = value;
        }

        if (user.References.Count > 0)
        {
            merged.References = user.References;
        }

        if (user.Templates.Count > 0)
        {
            merged.Templates = user.Templates;
        }

        return merged;
    }

    private static Dictionary<string, List<string>> CloneDictionary(
        Dictionary<string, List<string>> source)
    {
        return source.ToDictionary(
            pair => pair.Key,
            pair => new List<string>(pair.Value),
            StringComparer.OrdinalIgnoreCase);
    }
}
