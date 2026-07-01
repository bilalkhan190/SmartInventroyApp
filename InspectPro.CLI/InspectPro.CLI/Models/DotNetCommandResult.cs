namespace CaBootstrap.CLI.Models;

public sealed class DotNetCommandResult
{
    public required int ExitCode { get; init; }
    public required string StandardOutput { get; init; }
    public required string StandardError { get; init; }
    public required string Command { get; init; }

    public bool Succeeded => ExitCode == 0;
}
