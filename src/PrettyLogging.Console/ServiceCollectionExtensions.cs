using Microsoft.Extensions.Logging;
using PrettyLogging.Console;
using System;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static ILoggingBuilder AddPrettyConsole(this ILoggingBuilder loggingBuilder, Action<LoggingFormatterOptions> options)
    {
        loggingBuilder.Services.AddSingleton(_ => LogLevelReverseParser.Instance);

        loggingBuilder.AddConsole(options => options.FormatterName = LoggingFormatter.InternalName)
            .AddConsoleFormatter<LoggingFormatter, LoggingFormatterOptions>(options);
        return loggingBuilder;
    }
}
