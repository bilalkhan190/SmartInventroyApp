namespace CaBootstrap.CLI.Models;

public sealed class BootstrapResult
{
    public bool Succeeded { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = [];
    public IReadOnlyList<string> Warnings { get; init; } = [];

    public static BootstrapResult Success(IReadOnlyList<string>? warnings = null) =>
        new() { Succeeded = true, Warnings = warnings ?? [] };

    public static BootstrapResult Failure(params string[] errors) =>
        new() { Succeeded = false, Errors = errors };
}
