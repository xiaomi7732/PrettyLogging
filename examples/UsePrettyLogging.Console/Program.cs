// Pick one of the 2 options to see the demo.
#define IS_SIMPLE_EXAMPLE
// #undef IS_SIMPLE_EXAMPLE

using Microsoft.Extensions.Logging;

// Register
using ILoggerFactory factory = LoggerFactory.Create(builder =>
{
#if IS_SIMPLE_EXAMPLE
    builder.AddConsole().PrettyIt();
#else
    builder.AddConsole().PrettyIt(opt =>
    {
        opt.LogManagedThreadId = true;
        opt.IncludeScopes = true;
    });
#endif
});

// Create a logger as normal
ILogger logger = factory.CreateLogger<Program>();

// Use logger
using (logger.BeginScope("Scope"))
{
    using (logger.BeginScope("Scope2"))
    {
        logger.LogInformation("Hello {name}", "Pretty Logging");
    }
    logger.LogWarning("This is a warning!");
}