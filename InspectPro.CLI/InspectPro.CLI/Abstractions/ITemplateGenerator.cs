using CaBootstrap.CLI.Models;

namespace CaBootstrap.CLI.Abstractions;

public interface ITemplateGenerator
{
  IReadOnlyList<string> Generate(
    IReadOnlyList<ProjectInfo> projects,
    BootstrapConfiguration configuration,
    string configurationDirectory);
}
