
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Logging.AddPrettyConsole(opt =>
{
    opt.UseUtcTimestamp = true;
});

using (IHost host = builder.Build())
{
    ILogger logger = host.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Hello {name}", "Pretty Logging");
    logger.LogInformation("This is a warning!");
    await host.RunAsync();
}
