using CaBootstrap.CLI.Models;

namespace CaBootstrap.CLI.Abstractions;

public interface IReferenceInstaller
{
  Task<IReadOnlyList<string>> InstallAsync(
    IReadOnlyList<ProjectInfo> projects,
    BootstrapConfiguration configuration,
    CancellationToken cancellationToken = default);
}
