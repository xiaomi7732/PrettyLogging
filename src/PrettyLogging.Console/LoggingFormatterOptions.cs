using Microsoft.Extensions.Logging.Console;

namespace PrettyLogging.Console;

public class LoggingFormatterOptions : ConsoleFormatterOptions
{
    public LoggingFormatterOptions()
    {
        if (string.IsNullOrEmpty(TimestampFormat))
        {
            TimestampFormat = "o";
        }
    }

    public bool ShowLogLevel { get; set; } = true;

    public bool LogManagedThreadId { get; set; }

    public bool SingleLine { get; set; } = true;

    public LoggerCategoryMode CategoryMode { get; set; } = LoggerCategoryMode.Default;

    public LoggerColorBehavior ColorBehavior { get; set; } = LoggerColorBehavior.Default;
}