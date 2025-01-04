using Microsoft.Extensions.Logging;

// Register
using ILoggerFactory factory = LoggerFactory.Create(builder =>
{
    // Replace `AddConsole()` with `AddPrettyConsole()`.
    builder.AddConsole();
    // builder.AddPrettyConsole();
});

// Create a logger as normal
ILogger logger = factory.CreateLogger<Program>();

// Use logger
logger.LogInformation("Hello {name}", "Pretty Logging");
logger.LogInformation("This is a warning!");