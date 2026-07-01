using CaBootstrap.CLI.Models;

namespace CaBootstrap.CLI.Abstractions;

public interface IConfigurationService
{
  BootstrapConfiguration Load(string rootDirectory, string? configFilePath = null);
}
