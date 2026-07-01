using CaBootstrap.CLI.Utilities;

namespace CaBootstrap.CLI.Tests;

public class PackageVersionResolverTests
{
    [Theory]
    [InlineData("Microsoft.EntityFrameworkCore", "net8.0", "8.0.11")]
    [InlineData("Microsoft.AspNetCore.Authentication.JwtBearer", "net8.0", "8.0.11")]
    [InlineData("Asp.Versioning.Http", "net8.0", "8.0.11")]
    [InlineData("MediatR", "net8.0", null)]
    [InlineData("FluentValidation", "net8.0", null)]
    public void Resolve_MatchesTargetFramework(string packageId, string tfm, string? expected)
    {
        var version = PackageVersionResolver.Resolve(packageId, tfm);
        Assert.Equal(expected, version);
    }
}
