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
        string? message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception);
        if (message is null)
        {
            return;
        }

        Pretty(logEntry, scopeProvider, textWriter);
        textWriter.WriteLine(message);
    }

    private void Pretty<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
    {
        WritePrefix(logEntry, scopeProvider, textWriter);
    }

    private void WritePrefix<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
    {
        bool firstSection = true;

        if (!string.IsNullOrEmpty(_formatterOptions.TimestampFormat))
        {
            OutputSeparatorAndMutateFirstSection(textWriter, ref firstSection);
            textWriter.Write(string.Format($"{{0:{_formatterOptions.TimestampFormat}}}", _formatterOptions.UseUtcTimestamp ? DateTime.UtcNow : DateTime.Now));
        }

        if (_formatterOptions.DisplayLoggingLevel)
        {
            OutputSeparatorAndMutateFirstSection(textWriter, ref firstSection);
            textWriter.Write($"{_logLevelReverseParser.GetString(logEntry.LogLevel)}");
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