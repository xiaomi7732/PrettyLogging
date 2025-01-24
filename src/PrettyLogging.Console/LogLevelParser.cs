using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PrettyLogging.Console;

internal sealed class LogLevelReverseParser
{
    private LogLevelReverseParser() { }
    private static IDictionary<LogLevel, string> _lowerCaseMapping = CreateLowercaseMapping();
    private static IDictionary<LogLevel, string> _upperCaseMapping = CreateUppercaseMapping();

    // Order matters here: calling the constructor in a static initializer makes it run before
    // the static constructor. _mappings in this case needs to be put in front of it, because
    // the instance member of MaxWidth refers _mappings. Revert the order causes null reference
    // exception.
    public static LogLevelReverseParser Instance { get; } = new LogLevelReverseParser();

    public string GetString(LogLevel logLevel, LogLevelCase casing) => (casing switch
    {
        LogLevelCase.Upper => _upperCaseMapping,
        LogLevelCase.Lower => _lowerCaseMapping,
        _ => throw new NotSupportedException(),
    })[logLevel];

    public static int MaxWidth { get; } = _lowerCaseMapping.Values.Max(item => item.Length);

    private static Dictionary<LogLevel, string> CreateLowercaseMapping() => new()
    {
        [LogLevel.Critical] = "crit",
        [LogLevel.Error] = "error",
        [LogLevel.Warning] = "warn",
        [LogLevel.Information] = "info",
        [LogLevel.Debug] = "debug",
        [LogLevel.Trace] = "trace",
        [LogLevel.None] = "none",
    };

    public static Dictionary<LogLevel, string> CreateUppercaseMapping() => new()
    {
        [LogLevel.Critical] = "CRIT",
        [LogLevel.Error] = "ERROR",
        [LogLevel.Warning] = "WARN",
        [LogLevel.Information] = "INFO",
        [LogLevel.Debug] = "DEBUG",
        [LogLevel.Trace] = "TRACE",
        [LogLevel.None] = "NONE",
    };
}