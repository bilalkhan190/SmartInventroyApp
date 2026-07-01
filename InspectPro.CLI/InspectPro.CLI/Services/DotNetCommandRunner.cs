using System.Diagnostics;
using System.Text;
using CaBootstrap.CLI.Abstractions;
using CaBootstrap.CLI.Models;
using Microsoft.Extensions.Logging;

namespace CaBootstrap.CLI.Services;

public sealed class DotNetCommandRunner : IDotNetCommandRunner
{
    private readonly ILogger<DotNetCommandRunner> _logger;

    public DotNetCommandRunner(ILogger<DotNetCommandRunner> logger)
    {
        _logger = logger;
    }

    public Task<DotNetCommandResult> AddPackageAsync(
        string projectPath,
        string packageName,
        string? version = null,
        CancellationToken cancellationToken = default)
    {
        var args = new List<string> { "add", projectPath, "package", packageName };
        if (!string.IsNullOrWhiteSpace(version))
        {
            args.Add("--version");
            args.Add(version);
        }

        return RunAsync(Path.GetDirectoryName(projectPath)!, args, cancellationToken);
    }

    public Task<DotNetCommandResult> AddReferenceAsync(
        string fromProjectPath,
        string toProjectPath,
        CancellationToken cancellationToken = default)
    {
        var args = new List<string> { "add", fromProjectPath, "reference", toProjectPath };
        return RunAsync(Path.GetDirectoryName(fromProjectPath)!, args, cancellationToken);
    }

    public Task<DotNetCommandResult> RestoreAsync(
        string workingDirectory,
        CancellationToken cancellationToken = default) =>
        RunAsync(workingDirectory, ["restore"], cancellationToken);

    public Task<DotNetCommandResult> BuildAsync(
        string workingDirectory,
        CancellationToken cancellationToken = default) =>
        RunAsync(workingDirectory, ["build"], cancellationToken);

    public Task<DotNetCommandResult> TestAsync(
        string workingDirectory,
        CancellationToken cancellationToken = default) =>
        RunAsync(workingDirectory, ["test", "--no-build"], cancellationToken);

    public Task<DotNetCommandResult> EfMigrationsAddAsync(
        string startupProjectPath,
        string migrationName,
        string? contextProjectPath = null,
        CancellationToken cancellationToken = default)
    {
        var args = new List<string>
        {
            "ef", "migrations", "add", migrationName,
            "--project", contextProjectPath ?? startupProjectPath,
            "--startup-project", startupProjectPath
        };

        return RunAsync(Path.GetDirectoryName(startupProjectPath)!, args, cancellationToken);
    }

    public async Task<DotNetCommandResult> RunAsync(
        string workingDirectory,
        IReadOnlyList<string> arguments,
        CancellationToken cancellationToken = default)
    {
        var commandText = $"dotnet {string.Join(' ', arguments)}";
        _logger.LogDebug("Executing: {Command} in {Directory}", commandText, workingDirectory);

        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };

        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using var process = new Process { StartInfo = startInfo, EnableRaisingEvents = true };
        var stdout = new StringBuilder();
        var stderr = new StringBuilder();

        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data is not null)
            {
                stdout.AppendLine(e.Data);
                WriteColoredLine(e.Data, ConsoleColor.DarkGray);
            }
        };

        process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data is not null)
            {
                stderr.AppendLine(e.Data);
                WriteColoredLine(e.Data, ConsoleColor.Yellow);
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync(cancellationToken);

        return new DotNetCommandResult
        {
            ExitCode = process.ExitCode,
            StandardOutput = stdout.ToString(),
            StandardError = stderr.ToString(),
            Command = commandText
        };
    }

    private static void WriteColoredLine(string line, ConsoleColor color)
    {
        var previous = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(line);
        Console.ForegroundColor = previous;
    }
}
