using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using System.Runtime.Versioning;

namespace PrettyLogging.Console;

internal class LoggingFormatterConfigureOptions : IConfigureOptions<PrettyLoggingFormatterOptions>
{
    private readonly IConfiguration _configuration;

#if NET6_0_OR_GREATER
    [UnsupportedOSPlatform("browser")]
#endif
    public LoggingFormatterConfigureOptions(ILoggerProviderConfiguration<ConsoleLoggerProvider> providerConfiguration)
    {
        _configuration = providerConfiguration.GetFormatterOptionsSection();
    }

    public void Configure(PrettyLoggingFormatterOptions options) => options.Configure(_configuration);

}
