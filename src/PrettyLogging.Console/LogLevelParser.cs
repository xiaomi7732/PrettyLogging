using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace PrettyLogging.Console;

internal sealed class LogLevelReverseParser
{
    private LogLevelReverseParser() { }
    public static LogLevelReverseParser Instance { get; } = new LogLevelReverseParser();

    public static IDictionary<LogLevel, string> _mapping = new Dictionary<LogLevel, string>
    {
        [LogLevel.Critical] = "CRIT",
        [LogLevel.Error] = "ERR",
        [LogLevel.Warning] = "WARN",
        [LogLevel.Information] = "INFO",
        [LogLevel.Debug] = "DEBUG",
        [LogLevel.Trace] = "TRACE",
        [LogLevel.None] = "NONE",
    };

    public string GetString(LogLevel logLevel) => _mapping[logLevel];

    public int MaxWidth { get; } = "DEBUG".Length;
}