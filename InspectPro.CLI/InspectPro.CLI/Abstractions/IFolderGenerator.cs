using CaBootstrap.CLI.Models;

namespace CaBootstrap.CLI.Abstractions;

public interface IFolderGenerator
{
  IReadOnlyList<string> Generate(
    IReadOnlyList<ProjectInfo> projects,
    BootstrapConfiguration configuration);
}
