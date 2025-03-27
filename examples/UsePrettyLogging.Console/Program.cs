// Pick one of the 2 options to see the demo.
#define IS_SIMPLE_EXAMPLE
#undef IS_SIMPLE_EXAMPLE

using Microsoft.Extensions.Logging;
using UsePrettyLogging.ConsoleApp;

// Register
using ILoggerFactory factory = LoggerFactory.Create(configureLoggingFactory =>
{
#if IS_SIMPLE_EXAMPLE
    configureLoggingFactory.PrettyIt();
#else
    configureLoggingFactory.PrettyIt(opt =>
    {
        opt.ShowLogLevel = true;
        opt.ShowEventId = false;
        opt.ShowManagedThreadId = false;
        opt.SingleLine = true;
        opt.IncludeScopes = false;
        opt.ShowTimestamp = true;
        opt.LogLevelCase = PrettyLogging.Console.LogLevelCase.Upper;
        opt.CategoryMode = PrettyLogging.Console.LoggerCategoryMode.Short;
        opt.ColorBehavior = Microsoft.Extensions.Logging.Console.LoggerColorBehavior.Enabled;
        opt.UseUtcTimestamp = false;
        // opt.ApplySinglelineInMessage = false;
    });
#endif
    configureLoggingFactory.SetMinimumLevel(LogLevel.Trace);
});

// Create a logger as normal
ILogger logger = factory.CreateLogger("444.Program");

// Use logger
using (logger.BeginScope("Scope"))
{
    try
    {
        logger.LogTrace("This is a TRACE message.");
        logger.LogDebug("This is a DEBUG message.");
        using (logger.BeginScope("Scope2"))
        {
            logger.LogInformation("This is an INFO message.");
        }
        logger.LogWarning("This is a WARNING message!");
        logger.LogError("This is an ERROR message!");
        logger.LogCritical("This is a CRITICAL message!");

        Thrower.ThrowAnException();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "This error includes an exception!");
    }
}