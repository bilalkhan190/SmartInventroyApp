using CaBootstrap.CLI.Models;

namespace CaBootstrap.CLI.Utilities;

public static class ProjectRoleDetector
{
    private static readonly (ProjectRole Role, string[] Tokens)[] RoleTokens =
    [
        (ProjectRole.Api, ["Api", "Web", "Presentation"]),
        (ProjectRole.Application, ["Application"]),
        (ProjectRole.Infrastructure, ["Infrastructure"]),
        (ProjectRole.Domain, ["Domain"])
    ];

    public static ProjectRole Detect(string projectName)
    {
        foreach (var (role, tokens) in RoleTokens)
        {
            if (tokens.Any(token =>
                    projectName.Contains(token, StringComparison.OrdinalIgnoreCase)))
            {
                return role;
            }
        }

        return ProjectRole.Unknown;
    }

    public static ProjectRole ResolveRole(
        string projectName,
        IReadOnlyDictionary<string, string> configuredNames)
    {
        foreach (var (roleKey, configuredName) in configuredNames)
        {
            if (string.IsNullOrWhiteSpace(configuredName))
            {
                continue;
            }

            if (projectName.Equals(configuredName, StringComparison.OrdinalIgnoreCase))
            {
                return Enum.TryParse<ProjectRole>(roleKey, ignoreCase: true, out var role)
                    ? role
                    : ProjectRole.Unknown;
            }
        }

        return Detect(projectName);
    }
}
