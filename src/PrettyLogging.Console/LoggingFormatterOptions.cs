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

    public bool DisplayLoggingLevel { get; set; } = true;
}