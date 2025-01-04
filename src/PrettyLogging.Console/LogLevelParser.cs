using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace PrettyLogging.Console;

internal sealed class LogLevelReverseParser
{
    private LogLevelReverseParser() { }
    public static IDictionary<LogLevel, string> _mapping = CreateMapping();
    public static LogLevelReverseParser Instance { get; } = new LogLevelReverseParser();

    public string GetString(LogLevel logLevel) => _mapping[logLevel];

    public int MaxWidth { get; } = _mapping.Values.Max(item => item.Length);

    private static Dictionary<LogLevel, string> CreateMapping()
    {
        return new Dictionary<LogLevel, string>
        {
            [LogLevel.Critical] = "CRIT",
            [LogLevel.Error] = "ERR",
            [LogLevel.Warning] = "WARN",
            [LogLevel.Information] = "INFO",
            [LogLevel.Debug] = "DEBUG",
            [LogLevel.Trace] = "TRACE",
            [LogLevel.None] = "NONE",
        };
    }
}