using Microsoft.Extensions.Logging;

namespace CaBootstrap.CLI.Utilities;

public sealed class ProgressConsoleLogger : ILogger
{
    private readonly string _categoryName;

    public ProgressConsoleLogger(string categoryName)
    {
        _categoryName = categoryName;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Information;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var message = formatter(state, exception);
        var prefix = logLevel switch
        {
            LogLevel.Error or LogLevel.Critical => ("✖", ConsoleColor.Red),
            LogLevel.Warning => ("⚠", ConsoleColor.Yellow),
            _ => ("✔", ConsoleColor.Green)
        };

        var previousColor = Console.ForegroundColor;
        Console.ForegroundColor = prefix.Item2;
        Console.Write(prefix.Item1);
        Console.ForegroundColor = previousColor;
        Console.WriteLine($" {message}");

        if (exception is not null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(exception.Message);
            Console.ForegroundColor = previousColor;
        }
    }
}

public sealed class ProgressConsoleLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new ProgressConsoleLogger(categoryName);

    public void Dispose()
    {
    }
}
