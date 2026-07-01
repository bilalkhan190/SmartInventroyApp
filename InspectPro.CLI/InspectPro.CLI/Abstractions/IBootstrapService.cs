using CaBootstrap.CLI.Models;

namespace CaBootstrap.CLI.Abstractions;

public interface IBootstrapService
{
  Task<BootstrapResult> BootstrapAsync(
    string rootDirectory,
    string? configFilePath = null,
    CancellationToken cancellationToken = default);
}
