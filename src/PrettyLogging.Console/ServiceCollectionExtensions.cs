using Microsoft.Extensions.DependencyInjection;
using PrettyLogging.Console;
using System;

namespace Microsoft.Extensions.Logging;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a pretty console logger.
    /// </summary>
    public static ILoggingBuilder AddPrettyConsole(this ILoggingBuilder loggingBuilder, Action<LoggingFormatterOptions>? options = null)
    {
        loggingBuilder.Services.AddSingleton(_ => LogLevelReverseParser.Instance);

        loggingBuilder.AddConsole(options => options.FormatterName = LoggingFormatter.InternalName);
        if (options is null)
        {
            loggingBuilder.AddConsoleFormatter<LoggingFormatter, LoggingFormatterOptions>();
            return loggingBuilder;
        }
        loggingBuilder.AddConsoleFormatter<LoggingFormatter, LoggingFormatterOptions>(options);
        return loggingBuilder;
    }

    /// <summary>
    /// A syntax sugar method to append pretty formatter to console logger.
    /// </summary>
    public static ILoggingBuilder PrettyIt(this ILoggingBuilder loggingBuilder, Action<LoggingFormatterOptions>? options = null)
        => AddPrettyConsole(loggingBuilder, options);
}
