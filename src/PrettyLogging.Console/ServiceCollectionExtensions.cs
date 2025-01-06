using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Console;
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

        loggingBuilder.AddConsole(options =>
        {
#if DEBUG
            System.Console.WriteLine("Formatter name: {0}", options.FormatterName);
#endif
            if (string.IsNullOrEmpty(options.FormatterName) ||
                string.Equals(options.FormatterName, ConsoleFormatterNames.Simple, StringComparison.Ordinal))
            {
#if DEBUG
                System.Console.WriteLine("Overwriting formatter {0} => {1}.", options.FormatterName?? "(Null)", LoggingFormatter.InternalName);
#endif
                options.FormatterName = LoggingFormatter.InternalName;
            }
        });

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
    /// This will only take effect if no console formatter or only simple console formatter
    /// is specified. This won't have an effect on SystemD console formatter or Json console
    /// formatter.
    /// </summary>
    public static ILoggingBuilder PrettyIt(this ILoggingBuilder loggingBuilder, Action<LoggingFormatterOptions>? options = null)
        => AddPrettyConsole(loggingBuilder, options);
}
