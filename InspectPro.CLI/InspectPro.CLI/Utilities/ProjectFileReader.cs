using System.Text.RegularExpressions;

namespace CaBootstrap.CLI.Utilities;

public static partial class ProjectFileReader
{
    public static string? GetTargetFramework(string projectFilePath)
    {
        if (!File.Exists(projectFilePath))
        {
            return null;
        }

        var content = File.ReadAllText(projectFilePath);
        var match = TargetFrameworkRegex().Match(content);
        return match.Success ? match.Groups[1].Value : null;
    }

    public static bool UsesWebSdk(string projectFilePath)
    {
        if (!File.Exists(projectFilePath))
        {
            return false;
        }

        var firstLine = File.ReadLines(projectFilePath).FirstOrDefault() ?? string.Empty;
        return firstLine.Contains("Microsoft.NET.Sdk.Web", StringComparison.OrdinalIgnoreCase);
    }

    public static bool PackageReferenceExists(string projectFilePath, string packageId)
    {
        if (!File.Exists(projectFilePath))
        {
            return false;
        }

        var content = File.ReadAllText(projectFilePath);
        return content.Contains($"Include=\"{packageId}\"", StringComparison.OrdinalIgnoreCase);
    }

    [GeneratedRegex("<TargetFramework>([^<]+)</TargetFramework>", RegexOptions.IgnoreCase)]
    private static partial Regex TargetFrameworkRegex();
}
