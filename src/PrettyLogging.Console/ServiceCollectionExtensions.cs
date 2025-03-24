using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using PrettyLogging.Console;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

namespace Microsoft.Extensions.Logging;

public static class ServiceCollectionExtensions
{
    public static ILoggingBuilder AddTestConsole(this ILoggingBuilder builder) =>
            builder.AddFormatterWithName(LoggingFormatter.InternalName);

    /// <summary>
    /// Adds a pretty console logger.
    /// </summary>
    public static ILoggingBuilder AddPrettyConsole(this ILoggingBuilder loggingBuilder, Action<LoggingFormatterOptions>? options = null)
    {
        loggingBuilder.Services.AddSingleton(_ => LogLevelReverseParser.Instance);

        //         loggingBuilder.AddConsole(consoleOptions =>
        //         {
        // #if DEBUG
        //             System.Console.WriteLine("Formatter name: {0}", consoleOptions.FormatterName);
        // #endif
        //             if (string.IsNullOrEmpty(consoleOptions.FormatterName) ||
        //                 string.Equals(consoleOptions.FormatterName, ConsoleFormatterNames.Simple, StringComparison.Ordinal))
        //             {
        // #if DEBUG
        //                 System.Console.WriteLine("Overwriting formatter {0} => {1}.", consoleOptions.FormatterName ?? "(Null)", LoggingFormatter.InternalName);
        // #endif
        //                 consoleOptions.FormatterName = LoggingFormatter.InternalName;
        //                 // Default options to an empty action.

        //             }
        //         });

        options ??= (opt) => { };
        loggingBuilder.AddConsoleFormatter<LoggingFormatter, LoggingFormatterOptions, LoggingFormatterConfigureOptions>();
        loggingBuilder.AddConsoleWithFormatter(LoggingFormatter.InternalName, options);
        // loggingBuilder.AddConsoleFormatter<LoggingFormatter, LoggingFormatterOptions>(options);
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

    internal static ILoggingBuilder AddConsoleWithFormatter<TOptions>(this ILoggingBuilder builder, string name, Action<TOptions> configure)
        where TOptions : ConsoleFormatterOptions
    {
        if (configure is null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        builder.AddFormatterWithName(name);
        builder.Services.Configure(configure);

        return builder;
    }

    private static ILoggingBuilder AddFormatterWithName(this ILoggingBuilder builder, string name) =>
            builder.AddConsole((ConsoleLoggerOptions options) => options.FormatterName = name);

    private static ILoggingBuilder AddConsoleFormatter<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TFormatter,
        TOptions,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TConfigureOptions>(this ILoggingBuilder builder)
    where TOptions : ConsoleFormatterOptions
    where TFormatter : ConsoleFormatter
    where TConfigureOptions : class, IConfigureOptions<TOptions>
    {
        builder.AddConfiguration();

        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ConsoleFormatter, TFormatter>());
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<TOptions>, TConfigureOptions>());
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IOptionsChangeTokenSource<TOptions>, ConsoleLoggerFormatterOptionsChangeTokenSource<TFormatter, TOptions>>());

        return builder;
    }

    internal static IConfiguration GetFormatterOptionsSection(this ILoggerProviderConfiguration<ConsoleLoggerProvider> providerConfiguration)
    {
        return providerConfiguration.Configuration.GetSection("FormatterOptions");
    }
}

[UnsupportedOSPlatform("browser")]
internal sealed class ConsoleLoggerFormatterOptionsChangeTokenSource<TFormatter, TOptions> : ConfigurationChangeTokenSource<TOptions>
        where TOptions : ConsoleFormatterOptions
        where TFormatter : ConsoleFormatter
{
    public ConsoleLoggerFormatterOptionsChangeTokenSource(ILoggerProviderConfiguration<ConsoleLoggerProvider> providerConfiguration)
        : base(providerConfiguration.GetFormatterOptionsSection())
    {
    }
}
