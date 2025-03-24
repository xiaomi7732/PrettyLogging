using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using System.Runtime.Versioning;

namespace PrettyLogging.Console;

internal class LoggingFormatterConfigureOptions : IConfigureOptions<LoggingFormatterOptions>
{
            private readonly IConfiguration _configuration;
 
        [UnsupportedOSPlatform("browser")]
        public LoggingFormatterConfigureOptions(ILoggerProviderConfiguration<ConsoleLoggerProvider> providerConfiguration)
        {
            _configuration = providerConfiguration.GetFormatterOptionsSection();
        }
 
        public void Configure(LoggingFormatterOptions options) => options.Configure(_configuration);

}
