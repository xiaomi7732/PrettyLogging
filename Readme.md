# Pretty Logging

Make `Microsoft.Extensions.Logging` easier on the eyes!

## ⭐ me if you like it

If you like this project, please give it a star on the right upper corner to let others know!

## Getting started

1. Install the NuGet package:

    Run the following command to add the pre-release version of the package:

    ```shell
    dotnet add package PrettyLogging.Console --prerelease # TODO: Release a stable version.
    ```

    _(Note: A release will be available soon.)_

1. Set up Pretty Logging in your app

    Add the `PrettyIt()` method to your logger configuration:

    ```csharp
    builder.Logging.PrettyIt();
    ```

    For a full example, check out [Program.cs](./examples/UsePrettyLogging.WebAPI/Program.cs).

    If you are using this with an **Console application**, check out this [Program.cs](examples/UsePrettyLogging.Console/Program.cs).

1. Enjoy the clean output

    * Console application
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
            2025-01-03T16:28:30.2064911-08:00|WARN|This is a warning!
            ```

    * WebAPI:
        * Before:

            ```log
            info: Microsoft.Hosting.Lifetime[14]
                Now listening on: http://localhost:5140
            info: Microsoft.Hosting.Lifetime[0]
                Application started. Press Ctrl+C to shut down.
            info: Microsoft.Hosting.Lifetime[0]
                Hosting environment: Development
            info: Microsoft.Hosting.Lifetime[0]
                Content root path: C:\AIR\PrettyLogging\examples\UsePrettyLogging.WebAPI
            info: Microsoft.AspNetCore.Hosting.Diagnostics[1]
                Request starting HTTP/1.1 GET http://localhost:5140/ - - -
            info: Microsoft.AspNetCore.Routing.EndpointMiddleware[0]
                Executing endpoint 'HTTP: GET /'
            info: Microsoft.AspNetCore.Routing.EndpointMiddleware[1]
                Executed endpoint 'HTTP: GET /'
            info: Microsoft.AspNetCore.Hosting.Diagnostics[2]
                Request finished HTTP/1.1 GET http://localhost:5140/ - 200 - text/plain;+charset=utf-8 36.5664ms
            ```

        * After:

            ```log
            2025-01-03T17:56:08.7340978-08:00|INFO|Now listening on: http://localhost:5140
            2025-01-03T17:56:08.7401957-08:00|INFO|Application started. Press Ctrl+C to shut down.
            2025-01-03T17:56:08.7408854-08:00|INFO|Hosting environment: Development
            2025-01-03T17:56:08.7409246-08:00|INFO|Content root path: C:\AIR\PrettyLogging\examples\UsePrettyLogging.WebAPI
            2025-01-03T17:56:18.2800129-08:00|INFO|Request starting HTTP/1.1 GET http://localhost:5140/ - - -
            2025-01-03T17:56:18.3165023-08:00|INFO|Executing endpoint 'HTTP: GET /'
            2025-01-03T17:56:18.3196735-08:00|INFO|Executed endpoint 'HTTP: GET /'
            2025-01-03T17:56:18.3273233-08:00|INFO|Request finished HTTP/1.1 GET http://localhost:5140/ - 200 - text/plain;+charset=utf-8 41.9690ms
            ```
