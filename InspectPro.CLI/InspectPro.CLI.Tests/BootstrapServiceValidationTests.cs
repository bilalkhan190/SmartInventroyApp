using CaBootstrap.CLI.Models;
using CaBootstrap.CLI.Services;

namespace CaBootstrap.CLI.Tests;

public class BootstrapServiceValidationTests
{
    [Fact]
    public void ValidateStructure_WarnsWhenCoreProjectsMissing()
    {
        var projects = new List<ProjectInfo>
        {
            new()
            {
                Name = "Api",
                Role = ProjectRole.Api,
                DirectoryPath = "C:\\api",
                FilePath = "C:\\api\\Api.csproj"
            }
        };

        var warnings = BootstrapService.ValidateStructure(projects);

        Assert.Contains(warnings, w => w.Contains("Application"));
        Assert.Contains(warnings, w => w.Contains("Domain"));
    }
}
