# Pretty Logging

Makes `Microsoft.Extensions.Logging` easy for your eye!

## ⭐ me if you like it

Please star it if you like this project so that we all know!

## Getting started

* Add reference to NuGet package:

    ```shell
    dotnet add package PrettyLogging.Console --prerelease # TODO: Release a stable version.
    ```

* Call `AddPrettyConsole()` for your logging, in a Console application:

    ```csharp
    using Microsoft.Extensions.Logging;

    // Register
    using ILoggerFactory factory = LoggerFactory.Create(builder =>
    {
        // Replace `AddConsole()` with `AddPrettyConsole()`.
        builder.AddPrettyConsole(); // builder.AddConsole();
    });
    ```

    See [Program.cs](examples/UsePrettyLogging.Console/Program.cs) for a full example.

* Enjoy the cleaning output:

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
