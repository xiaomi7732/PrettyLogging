# Options for customizations

PrettyLogging offers beautifully formatted, easy-to-read logs by default. We recognize that every project has unique requirements, which is why our library is designed to be highly customizable. Whether you need something simple or tailored to your specific style, adjusting the log format to suit your needs is quick and straightforward.

## Options available

| Option                     | Description                                                             | Default Value               | Available Values                                                   |
|----------------------------|-------------------------------------------------------------------------|-----------------------------|--------------------------------------------------------------------|
| `SingleLine`                | Whether to output the log in a single line format.                       | `true`                      | `true`, `false`                                                    |
| `ShowTimestamp`             | Whether to include a timestamp in the log entry.                         | `true`                      | `true`, `false`                                                    |
| `ShowLogLevel`              | Whether to include the log level in the log entry.                       | `true`                      | `true`, `false`                                                    |
| `ShowManagedThreadId`       | Whether to show the managed thread ID in the log entry.                  | `false`                     | `true`, `false`                                                    |
| `ShowEventId`               | Whether to display the event ID in the log entry.                        | `false`                     | `true`, `false`                                                    |
| `LogLevelCase`              | Defines the case format for the log level.                              | `LogLevelCase.Upper`        | `LogLevelCase.Upper`, `LogLevelCase.Lower`                           |
| `CategoryMode`              | Specifies how the logger category is displayed.                          | `LoggerCategoryMode.None`   | `LoggerCategoryMode.None`, `LoggerCategoryMode.Short`, `LoggerCategoryMode.Full` |
| `ColorBehavior`             | Defines the color behavior of the log output.                            | `LoggerColorBehavior.Default`| `LoggerColorBehavior.Default`, `LoggerColorBehavior.Enabled`, `LoggerColorBehavior.Disabled` |

Refer to [source file](../src/PrettyLogging.Console/LoggingFormatterOptions.cs) for actual options.

## How to configure

### By code

```csharp
...
// Set on the delegate of PrettyIt:
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
```

### By `appsettings.json`

// TODO: Document
