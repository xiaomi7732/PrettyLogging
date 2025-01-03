using Microsoft.Extensions.Logging;
using PrettyLogging.Console;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static ILoggingBuilder AddPrettyConsole(this ILoggingBuilder loggingBuilder)
    {
        loggingBuilder.Services.AddSingleton(_ => LogLevelReverseParser.Instance);

        loggingBuilder.AddConsole(options => options.FormatterName = LoggingFormatter.InternalName)
            .AddConsoleFormatter<LoggingFormatter, LoggingFormatterOptions>();
        return loggingBuilder;
    }
}
