using CaBootstrap.CLI.Models;

namespace CaBootstrap.CLI.Abstractions;

public interface IDotNetCommandRunner
{
  Task<DotNetCommandResult> RunAsync(
    string workingDirectory,
    IReadOnlyList<string> arguments,
    CancellationToken cancellationToken = default);

  Task<DotNetCommandResult> AddPackageAsync(
    string projectPath,
    string packageName,
    string? version = null,
    CancellationToken cancellationToken = default);

  Task<DotNetCommandResult> AddReferenceAsync(
    string fromProjectPath,
    string toProjectPath,
    CancellationToken cancellationToken = default);

  Task<DotNetCommandResult> RestoreAsync(
    string workingDirectory,
    CancellationToken cancellationToken = default);

  Task<DotNetCommandResult> BuildAsync(
    string workingDirectory,
    CancellationToken cancellationToken = default);

  Task<DotNetCommandResult> TestAsync(
    string workingDirectory,
    CancellationToken cancellationToken = default);

  Task<DotNetCommandResult> EfMigrationsAddAsync(
    string startupProjectPath,
    string migrationName,
    string? contextProjectPath = null,
    CancellationToken cancellationToken = default);
}
