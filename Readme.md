# Pretty Logging

Make `Microsoft.Extensions.Logging` easier on the eyes!

## ⭐ me if you like it

If you like this project, please give it a star to let others know!

## Getting started

1. Install the NuGet package:

    Run the following command to add the pre-release version of the package:

    ```shell
    dotnet add package PrettyLogging.Console --prerelease # TODO: Release a stable version.
    ```

    _(Note: A release will be available soon.)_

1. Set up Pretty Logging in your Console app

    Add the `AddPrettyConsole()` method to your logger configuration:

    ```csharp
    using Microsoft.Extensions.Logging;

    // Register
    using ILoggerFactory factory = LoggerFactory.Create(builder =>
    {
        // Replace `AddConsole()` with `AddPrettyConsole()`.
        builder.AddPrettyConsole(); // builder.AddConsole();
    });
    ```

    For a full example, check out [Program.cs](examples/UsePrettyLogging.Console/Program.cs).

1. Enjoy the clean output

    * Before:

    ```log
    info: Program[0]
        Hello Pretty Logging
    info: Program[0]
        This is a warning!
    ```

    * After:

    ```log
    2025-01-03T16:28:30.2001407-08:00|INFO|Hello Pretty Logging
    2025-01-03T16:28:30.2064911-08:00|INFO|This is a warning!
    ```
