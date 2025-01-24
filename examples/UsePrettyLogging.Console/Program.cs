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
    logger.LogTrace("Trace it!");
    logger.LogDebug("Log Debug!");
    using (logger.BeginScope("Scope2"))
    {
        logger.LogInformation("Hello {name}", "Pretty Logging");
    }
    logger.LogWarning("This is a warning!");
    logger.LogError(new InvalidOperationException(), "Demo of an exception!");
    logger.LogCritical("Critical error!");
}