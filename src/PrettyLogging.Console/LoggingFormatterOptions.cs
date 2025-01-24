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

    public bool ShowEventId { get; set; } = false;

    public bool ShowManagedThreadId { get; set; }

    public bool SingleLine { get; set; } = true;

    public LogLevelCase LogLevelCase { get; set; } = LogLevelCase.Upper;

    public LoggerCategoryMode CategoryMode { get; set; } = LoggerCategoryMode.None;

    public LoggerColorBehavior ColorBehavior { get; set; } = LoggerColorBehavior.Default;
}