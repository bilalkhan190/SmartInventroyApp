namespace CaBootstrap.CLI.Models;

public sealed class BootstrapConfiguration
{
    public Dictionary<string, string> ProjectNames { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public Dictionary<string, List<string>> Packages { get; set; } =
        new(StringComparer.OrdinalIgnoreCase);

    public Dictionary<string, List<string>> Folders { get; set; } =
        new(StringComparer.OrdinalIgnoreCase);

    public List<ReferenceRule> References { get; set; } = [];

    public List<TemplateDefinition> Templates { get; set; } = [];

    public BootstrapFeatures Features { get; set; } = new();
}

public sealed class ReferenceRule
{
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
}

public sealed class TemplateDefinition
{
    public string Name { get; set; } = string.Empty;
    public string TargetProject { get; set; } = string.Empty;
    public string RelativePath { get; set; } = string.Empty;
    public string? TemplateResource { get; set; }
    public string? TemplateFile { get; set; }
}

public sealed class BootstrapFeatures
{
    public bool InstallPackages { get; set; } = true;
    public bool AddReferences { get; set; } = true;
    public bool CreateFolders { get; set; } = true;
    public bool GenerateTemplates { get; set; } = true;
    public bool Restore { get; set; } = true;
    public bool Build { get; set; } = true;
    public bool RunTests { get; set; }
}
