using CaBootstrap.CLI.Models;

namespace CaBootstrap.CLI.Abstractions;

public interface IProjectScanner
{
  IReadOnlyList<ProjectInfo> Scan(string rootDirectory, BootstrapConfiguration configuration);
}
