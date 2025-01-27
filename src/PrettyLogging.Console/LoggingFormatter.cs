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
    private const string _separator = "|";
    private const string LoglevelPadding = ": ";
    private static string _messagePadding = new(' ', LogLevelReverseParser.MaxWidth + LoglevelPadding.Length);
    private static readonly string _newLineWithMessagePadding = Environment.NewLine + _messagePadding;

#if NET
    private static bool IsAndroidOrAppleMobile => OperatingSystem.IsAndroid() ||
                                                    OperatingSystem.IsTvOS() ||
                                                    OperatingSystem.IsIOS(); // returns true on MacCatalyst
#else
    private static bool IsAndroidOrAppleMobile => false;
#endif

    private bool _isDisposed;
    private readonly IDisposable? _optionsReloadToken;
    private readonly LogLevelReverseParser _logLevelReverseParser;
    private LoggingFormatterOptions _formatterOptions;

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
        string message = logEntry.Formatter(logEntry.State, logEntry.Exception);

#if NET9_0_OR_GREATER
        if (logEntry.State is BufferedLogRecord bufferedRecord)
        {
            message = bufferedRecord.FormattedMessage ?? string.Empty;
            WriteInternal(null, textWriter, message, bufferedRecord.LogLevel, bufferedRecord.EventId.Id, bufferedRecord.Exception, logEntry.Category, bufferedRecord.Timestamp);
            return;
        }
#endif

        if (logEntry.Exception is null && string.IsNullOrEmpty(message))
        {
            return;
        }

        // We extract most of the work into a non-generic method to save code size. If this was left in the generic
        // method, we'd get generic specialization for all TState parameters, but that's unnecessary.
        WriteInternal(scopeProvider, textWriter, message, logEntry.LogLevel, logEntry.EventId.Id, logEntry.Exception?.ToString(), logEntry.Category, GetCurrentDateTime());
        return;
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
            textWriter.Write(_separator);
        }

        if (logLevelString != null)
        {
            WriteColoredMessage(textWriter, logLevelString, logLevelColors.Background, logLevelColors.Foreground, LogLevelReverseParser.MaxWidth);
            textWriter.Write(_separator);
        }

        bool singleLine = _formatterOptions.SingleLine;

        // Example:
        // info: ConsoleApp.Program[10]
        //       Request received

        // category and event id
        if (WriteCategory(textWriter, category))
        {
            textWriter.Write(_separator);
        }

        if (_formatterOptions.ShowEventId)
        {
#if NET
            Span<char> span = stackalloc char[10];
            if (eventId.TryFormat(span, out int charsWritten))
                textWriter.Write(span.Slice(0, charsWritten));
            else
#endif
                textWriter.Write(eventId.ToString());

            textWriter.Write(_separator);
        }

        if (!singleLine)
        {
            textWriter.Write(Environment.NewLine);
        }

        if (_formatterOptions.ShowManagedThreadId)
        {
            textWriter.Write($"{Thread.CurrentThread.ManagedThreadId}");
            textWriter.Write(_separator);
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

    private bool WriteCategory(TextWriter textWriter, string category)
    {
        if (_formatterOptions.CategoryMode == LoggerCategoryMode.None)
        {
            return false;
        }

        string effectiveCategory = category;
        if (_formatterOptions.CategoryMode == LoggerCategoryMode.Short)
        {
            int lastIndexOfPeriod = category.LastIndexOf('.');
            if (lastIndexOfPeriod >= 0)
            {
                effectiveCategory = category.Substring(lastIndexOfPeriod + 1);
            }
        }

        textWriter.Write(effectiveCategory);
        return effectiveCategory.Length > 0;
    }

    private static void WriteMessage(TextWriter textWriter, string message, bool singleLine)
    {
        if (!string.IsNullOrEmpty(message))
        {
            if (singleLine)
            {
                textWriter.Write(' ');
                WriteReplacing(textWriter, Environment.NewLine, " ", message);
            }
            else
            {
                textWriter.Write(_messagePadding);
                WriteReplacing(textWriter, Environment.NewLine, _newLineWithMessagePadding, message);
                textWriter.Write(Environment.NewLine);
            }
        }

        static void WriteReplacing(TextWriter writer, string oldValue, string newValue, string message)
        {
            string newMessage = message.Replace(oldValue, newValue);
            writer.Write(newMessage);
        }
    }

    private ConsoleColors GetLogLevelConsoleColors(LogLevel logLevel)
    {
        // We shouldn't be outputting color codes for Android/Apple mobile platforms,
        // they have no shell (adb shell is not meant for running apps) and all the output gets redirected to some log file.
        bool disableColors = (_formatterOptions.ColorBehavior == LoggerColorBehavior.Disabled) ||
            (_formatterOptions.ColorBehavior == LoggerColorBehavior.Default && (!ConsoleUtils.EmitAnsiColorCodes || IsAndroidOrAppleMobile));
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

    private void WriteScopeInformation(TextWriter textWriter, IExternalScopeProvider? scopeProvider, bool singleLine)
    {
        if (_formatterOptions.IncludeScopes && scopeProvider is not null)
        {
            bool paddingNeeded = !singleLine;
            scopeProvider.ForEachScope((scope, state) =>
            {
                if (paddingNeeded)
                {
                    paddingNeeded = false;
                    state.Write(_messagePadding);
                    state.Write("=> ");
                }
                else
                {
                    state.Write(" => ");
                }
                state.Write(scope);
            }, textWriter);

            if (!paddingNeeded && !singleLine)
            {
                textWriter.Write(Environment.NewLine);
            }
        }
    }

    private string GetLogLevelString(LogLevel logLevel) => _logLevelReverseParser.GetString(logLevel, _formatterOptions.LogLevelCase);

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

    private static void WriteColoredMessage(TextWriter textWriter, string message, ConsoleColor? background, ConsoleColor? foreground, int width)
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

        string paddedString = message;
        if (width > 0)
        {
            paddedString = message.PadRight(width);
        }
        textWriter.Write(paddedString);
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