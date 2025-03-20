using Microsoft.Extensions.Logging.Console;

namespace PrettyLogging.Console;

public class LoggingFormatterOptions : ConsoleFormatterOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to show the log level.
    /// </summary>
    public bool ShowLogLevel { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to show the event ID.
    /// </summary>
    public bool ShowEventId { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether to show the managed thread ID.
    /// </summary>
    public bool ShowManagedThreadId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to format logs in a single line.
    /// </summary>
    public bool SingleLine { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to show the timestamp.
    /// </summary>
    public bool ShowTimestamp { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to apply single-line formatting to messages. Defaults to false.
    /// When set to true, newlines are replaced with spaces in "SingleLine" mode.
    /// When set to false, newlines in the message are preserved, even if "SingleLine" is true.
    /// This setting has no effect if "SingleLine" is false.
    /// </summary>
    public bool ApplySinglelineInMessage { get; set; } = false;

    /// <summary>
    /// Gets or sets the case of the log level.
    /// </summary>
    public LogLevelCase LogLevelCase { get; set; } = LogLevelCase.Upper;

    /// <summary>
    /// Gets or sets the mode for the logger category.
    /// </summary>
    public LoggerCategoryMode CategoryMode { get; set; } = LoggerCategoryMode.None;

    /// <summary>
    /// Gets or sets the color behavior for the logger.
    /// </summary>
    public LoggerColorBehavior ColorBehavior { get; set; } = LoggerColorBehavior.Default;
}