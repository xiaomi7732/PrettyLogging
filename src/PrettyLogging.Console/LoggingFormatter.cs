using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading;

namespace PrettyLogging.Console;

internal class LoggingFormatter : ConsoleFormatter, IDisposable
{
    public const string InternalName = "PrettyLogging.Console";

    private bool _isDisposed;
    private readonly IDisposable? _optionsReloadToken;
    private readonly LogLevelReverseParser _logLevelReverseParser;
    LoggingFormatterOptions _formatterOptions;

    public LoggingFormatter(
        IOptionsMonitor<LoggingFormatterOptions> options,
        LogLevelReverseParser logLevelReverseParser
    ) : base(InternalName)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        _optionsReloadToken = options.OnChange(OnOptionsChange);
        _formatterOptions = options.CurrentValue;
        _logLevelReverseParser = logLevelReverseParser ?? throw new ArgumentNullException(nameof(logLevelReverseParser));
    }

    private void OnOptionsChange(LoggingFormatterOptions options) => _formatterOptions = options;

    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
    {
        if (logEntry.State is BufferedLogRecord bufferedRecord)
        {
            string message = bufferedRecord.FormattedMessage ?? string.Empty;
            WriteInternal(null, textWriter, message, bufferedRecord.LogLevel, bufferedRecord.EventId.Id, bufferedRecord.Exception, logEntry.Category, bufferedRecord.Timestamp);
        }
        else
        {
            string message = logEntry.Formatter(logEntry.State, logEntry.Exception);
            if (logEntry.Exception is null && string.IsNullOrEmpty(message))
            {
                return;
            }

            // We extract most of the work into a non-generic method to save code size. If this was left in the generic
            // method, we'd get generic specialization for all TState parameters, but that's unnecessary.
            WriteInternal(scopeProvider, textWriter, message, logEntry.LogLevel, logEntry.EventId.Id, logEntry.Exception?.ToString(), logEntry.Category, GetCurrentDateTime());
        }


        // string? message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception);
        // if (message is null)
        // {
        //     return;
        // }
    }

    private void WriteInternal(IExternalScopeProvider? scopeProvider, TextWriter textWriter, string message, LogLevel logLevel,
            int eventId, string? exception, string category, DateTimeOffset stamp)
    {
        ConsoleColors logLevelColors = GetLogLevelConsoleColors(logLevel);
        string logLevelString = GetLogLevelString(logLevel);

        string? timestamp = null;
        string? timestampFormat = _formatterOptions.TimestampFormat;
        if (timestampFormat != null)
        {
            timestamp = stamp.ToString(timestampFormat);
        }
        if (timestamp != null)
        {
            textWriter.Write(timestamp);
        }
        if (logLevelString != null)
        {
            textWriter.WriteColoredMessage(logLevelString, logLevelColors.Background, logLevelColors.Foreground);
        }

        bool singleLine = _formatterOptions.SingleLine;

        // Example:
        // info: ConsoleApp.Program[10]
        //       Request received

        // category and event id
        textWriter.Write(LoglevelPadding);
        textWriter.Write(category);
        textWriter.Write('[');

#if NET
            Span<char> span = stackalloc char[10];
            if (eventId.TryFormat(span, out int charsWritten))
                textWriter.Write(span.Slice(0, charsWritten));
            else
#endif
        textWriter.Write(eventId.ToString());

        textWriter.Write(']');
        if (!singleLine)
        {
            textWriter.Write(Environment.NewLine);
        }

        // scope information
        WriteScopeInformation(textWriter, scopeProvider, singleLine);
        WriteMessage(textWriter, message, singleLine);

        // Example:
        // System.InvalidOperationException
        //    at Namespace.Class.Function() in File:line X
        if (exception != null)
        {
            // exception message
            WriteMessage(textWriter, exception, singleLine);
        }
        if (singleLine)
        {
            textWriter.Write(Environment.NewLine);
        }
    }

    private ConsoleColors GetLogLevelConsoleColors(LogLevel logLevel)
    {
        // We shouldn't be outputting color codes for Android/Apple mobile platforms,
        // they have no shell (adb shell is not meant for running apps) and all the output gets redirected to some log file.
        bool disableColors = (FormatterOptions.ColorBehavior == LoggerColorBehavior.Disabled) ||
            (FormatterOptions.ColorBehavior == LoggerColorBehavior.Default && (!ConsoleUtils.EmitAnsiColorCodes || IsAndroidOrAppleMobile));
        if (disableColors)
        {
            return new ConsoleColors(null, null);
        }
        // We must explicitly set the background color if we are setting the foreground color,
        // since just setting one can look bad on the users console.
        return logLevel switch
        {
            LogLevel.Trace => new ConsoleColors(ConsoleColor.Gray, ConsoleColor.Black),
            LogLevel.Debug => new ConsoleColors(ConsoleColor.Gray, ConsoleColor.Black),
            LogLevel.Information => new ConsoleColors(ConsoleColor.DarkGreen, ConsoleColor.Black),
            LogLevel.Warning => new ConsoleColors(ConsoleColor.Yellow, ConsoleColor.Black),
            LogLevel.Error => new ConsoleColors(ConsoleColor.Black, ConsoleColor.DarkRed),
            LogLevel.Critical => new ConsoleColors(ConsoleColor.White, ConsoleColor.DarkRed),
            _ => new ConsoleColors(null, null)
        };
    }

    private void WriteSuffix<TState>(LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
    {
        // TODO: Adding more customizations
        return;
    }

    private void WritePrefix<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
    {
        bool firstSection = true;

        if (!string.IsNullOrEmpty(_formatterOptions.TimestampFormat))
        {
            OutputSeparatorAndMutateFirstSection(textWriter, ref firstSection);
            textWriter.Write(string.Format($"{{0:{_formatterOptions.TimestampFormat}}}", _formatterOptions.UseUtcTimestamp ? DateTime.UtcNow : DateTime.Now));
        }

        if (_formatterOptions.ShowLogLevel)
        {
            OutputSeparatorAndMutateFirstSection(textWriter, ref firstSection);
            textWriter.Write(_logLevelReverseParser.GetString(logEntry.LogLevel).PadRight(_logLevelReverseParser.MaxWidth, ' '));
        }

        if (_formatterOptions.LogManagedThreadId)
        {
            OutputSeparatorAndMutateFirstSection(textWriter, ref firstSection);
            textWriter.Write($"{Thread.CurrentThread.ManagedThreadId}");
        }

        if (!firstSection)
        {
            OutputSeparatorAndMutateFirstSection(textWriter, ref firstSection);
        }

        if (_formatterOptions.IncludeScopes && scopeProvider is not null)
        {
            bool writeScope = false;
            scopeProvider.ForEachScope((obj, state) =>
            {
                if (obj is null)
                {
                    return;
                }
                writeScope = true;
                textWriter.Write($"=>{obj}");
            }, logEntry.State);

            if (writeScope)
            {
                bool isFirstSection = false;
                OutputSeparatorAndMutateFirstSection(textWriter, ref isFirstSection);
            }
        }
    }

    private void OutputSeparatorAndMutateFirstSection(TextWriter textWriter, ref bool isFirstSection)
    {
        if (!isFirstSection)
        {
            textWriter.Write("|");
        }
        isFirstSection = false;
    }

    private string GetLogLevelString(LogLevel logLevel) => _logLevelReverseParser.GetString(logLevel);

    private readonly struct ConsoleColors
    {
        public ConsoleColors(ConsoleColor? foreground, ConsoleColor? background)
        {
            Foreground = foreground;
            Background = background;
        }

        public ConsoleColor? Foreground { get; }

        public ConsoleColor? Background { get; }
    }

    private DateTimeOffset GetCurrentDateTime() => _formatterOptions.TimestampFormat != null
            ? (_formatterOptions.UseUtcTimestamp ? DateTimeOffset.UtcNow : DateTimeOffset.Now)
            : DateTimeOffset.MinValue;

    private static void WriteColoredMessage(TextWriter textWriter, string message, ConsoleColor? background, ConsoleColor? foreground)
    {
        // Order: backgroundcolor, foregroundcolor, Message, reset foregroundcolor, reset backgroundcolor
        if (background.HasValue)
        {
            textWriter.Write(AnsiParser.GetBackgroundColorEscapeCode(background.Value));
        }
        if (foreground.HasValue)
        {
            textWriter.Write(AnsiParser.GetForegroundColorEscapeCode(foreground.Value));
        }
        textWriter.Write(message);
        if (foreground.HasValue)
        {
            textWriter.Write(AnsiParser.DefaultForegroundColor); // reset to default foreground color
        }
        if (background.HasValue)
        {
            textWriter.Write(AnsiParser.DefaultBackgroundColor); // reset to the background color
        }
    }

    protected virtual void Dispose(bool isDisposing)
    {
        if (!_isDisposed)
        {
            if (isDisposing)
            {
                _optionsReloadToken?.Dispose();
            }
        }
        _isDisposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}