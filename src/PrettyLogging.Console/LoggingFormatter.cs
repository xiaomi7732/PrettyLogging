using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using System;
using System.IO;

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
        string? message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception);
        if (message is null)
        {
            return;
        }

        Pretty(logEntry, textWriter);

        textWriter.WriteLine(message);
    }

    private void Pretty<TState>(in LogEntry<TState> logEntry, TextWriter textWriter)
    {
        WritePrefix(logEntry, textWriter);
    }

    private void WritePrefix<TState>(in LogEntry<TState> logEntry, TextWriter textWriter)
    {
        if (_formatterOptions.DisplayLoggingLevel)
        {
            textWriter.Write($"[{_logLevelReverseParser.GetString(logEntry.LogLevel)}] ");
        }

        if (!string.IsNullOrEmpty(_formatterOptions.TimestampFormat))
        {
            textWriter.Write(string.Format($"{{0:{_formatterOptions.TimestampFormat}}} ", _formatterOptions.UseUtcTimestamp ? DateTime.UtcNow : DateTime.Now));
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