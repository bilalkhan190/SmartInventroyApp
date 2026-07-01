namespace CaBootstrap.CLI.Utilities;

public static class PackageVersionResolver
{
    private static readonly HashSet<string> MicrosoftVersionedPrefixes =
    [
        "Microsoft.EntityFrameworkCore",
        "Microsoft.AspNetCore",
        "Microsoft.Extensions.Hosting",
        "Asp.Versioning"
    ];

    public static string? Resolve(string packageId, string? targetFramework)
    {
        if (string.IsNullOrWhiteSpace(targetFramework))
        {
            return null;
        }

        if (!targetFramework.StartsWith("net", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var versionSegment = targetFramework[3..];
        if (!Version.TryParse(versionSegment, out var tfmVersion))
        {
            return null;
        }

        var major = tfmVersion.Major;
        if (major <= 0)
        {
            return null;
        }

        if (!RequiresPinnedVersion(packageId))
        {
            return null;
        }

        return packageId switch
        {
            "HealthChecks.UI.Client" => major switch
            {
                8 => "8.0.1",
                _ => null
            },
            _ => $"{major}.0.11"
        };
    }

    private static bool RequiresPinnedVersion(string packageId)
    {
        return MicrosoftVersionedPrefixes.Any(prefix =>
            packageId.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
    }
}
