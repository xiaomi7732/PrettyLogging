// Pick one of the 2 options to see the demo.
#define IS_SIMPLE_EXAMPLE
// #undef IS_SIMPLE_EXAMPLE

using Microsoft.Extensions.Logging;

// Register
using ILoggerFactory factory = LoggerFactory.Create(builder =>
{
#if IS_SIMPLE_EXAMPLE
    builder.AddSimpleConsole().PrettyIt();
#else
    builder.AddSimpleConsole().PrettyIt(opt =>
    {
        opt.ShowLogLevel = true;
        opt.ShowEventId = false;
        opt.ShowManagedThreadId = false;
        opt.SingleLine = true;
        opt.IncludeScopes = false;
        opt.ShowTimestamp = false;
        opt.LogLevelCase = PrettyLogging.Console.LogLevelCase.Upper;
        opt.CategoryMode = PrettyLogging.Console.LoggerCategoryMode.None;
        opt.ColorBehavior = Microsoft.Extensions.Logging.Console.LoggerColorBehavior.Enabled;
    
    });
#endif

    builder.SetMinimumLevel(LogLevel.Trace);
});

// Create a logger as normal
ILogger logger = factory.CreateLogger("444.Program");

// Use logger
using (logger.BeginScope("Scope"))
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
    logger.LogError(new InvalidOperationException(), "This error includes an exception!");
}