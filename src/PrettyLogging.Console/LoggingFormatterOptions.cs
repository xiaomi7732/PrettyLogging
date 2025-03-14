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

    public bool ShowTimestamp { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to apply singleline in messages. Default to false.
    /// When set to false, newline in the message will be honored, even when "SingleLine" is set to true.
    /// When set to false, newline will be replaced by a space in "SingleLine" mode.
    /// This value has no impact when "SingleLine" is set to false.
    /// </summary>
    public bool ApplySinglelineInMessage { get; set; } = false;

    public LogLevelCase LogLevelCase { get; set; } = LogLevelCase.Upper;

    public LoggerCategoryMode CategoryMode { get; set; } = LoggerCategoryMode.None;

    public LoggerColorBehavior ColorBehavior { get; set; } = LoggerColorBehavior.Default;
}