using System.Reflection;
using CaBootstrap.CLI.Abstractions;
using CaBootstrap.CLI.Models;
using Microsoft.Extensions.Logging;

namespace CaBootstrap.CLI.Installers;

public sealed class TemplateGenerator : ITemplateGenerator
{
    private readonly ILogger<TemplateGenerator> _logger;
    private readonly Assembly _assembly;

    public TemplateGenerator(ILogger<TemplateGenerator> logger)
    {
        _logger = logger;
        _assembly = Assembly.GetExecutingAssembly();
    }

    public IReadOnlyList<string> Generate(
        IReadOnlyList<ProjectInfo> projects,
        BootstrapConfiguration configuration,
        string configurationDirectory)
    {
        var created = new List<string>();
        var byRole = projects
            .Where(p => p.Role != ProjectRole.Unknown)
            .GroupBy(p => p.Role)
            .ToDictionary(g => g.Key, g => g.First());

        foreach (var template in configuration.Templates)
        {
            if (!Enum.TryParse<ProjectRole>(template.TargetProject, ignoreCase: true, out var role))
            {
                _logger.LogWarning("Unknown target project role for template: {Name}", template.Name);
                continue;
            }

            if (!byRole.TryGetValue(role, out var project))
            {
                _logger.LogWarning(
                    "Skipping template {Name}: project role {Role} not found",
                    template.Name,
                    template.TargetProject);
                continue;
            }

            var targetPath = Path.Combine(project.DirectoryPath, template.RelativePath);
            if (File.Exists(targetPath))
            {
                continue;
            }

            var content = ResolveTemplateContent(template, configurationDirectory);
            if (content is null)
            {
                _logger.LogWarning("Template content not found for: {Name}", template.Name);
                continue;
            }

            var directory = Path.GetDirectoryName(targetPath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var rendered = content
                .Replace("{{RootNamespace}}", project.Name, StringComparison.Ordinal)
                .Replace("{{ProjectName}}", project.Name, StringComparison.Ordinal);

            File.WriteAllText(targetPath, rendered);
            created.Add(targetPath);
            _logger.LogInformation("Generated template file: {Path}", targetPath);
        }

        return created;
    }

    private string? ResolveTemplateContent(TemplateDefinition template, string configurationDirectory)
    {
        if (!string.IsNullOrWhiteSpace(template.TemplateFile))
        {
            var filePath = Path.IsPathRooted(template.TemplateFile)
                ? template.TemplateFile
                : Path.Combine(configurationDirectory, template.TemplateFile);

            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }
        }

        if (!string.IsNullOrWhiteSpace(template.TemplateResource))
        {
            using var stream = _assembly.GetManifestResourceStream(template.TemplateResource);
            if (stream is not null)
            {
                using var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }

        var shippedTemplatePath = Path.Combine(
            AppContext.BaseDirectory,
            "Templates",
            Path.GetFileName(template.TemplateResource ?? $"{template.Name}.cs.template"));
        if (File.Exists(shippedTemplatePath))
        {
            return File.ReadAllText(shippedTemplatePath);
        }

        return null;
    }
}
