using Microsoft.Extensions.Logging.Console;

namespace PrettyLogging.Console;

public class LoggingFormatterOptions : ConsoleFormatterOptions
{
    public LoggingFormatterOptions()
    {
        if (string.IsNullOrEmpty(TimestampFormat))
        {
            TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff";
        }
    }

    public bool DisplayLoggingLevel { get; set; } = true;
}