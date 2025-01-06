#!/bin/bash
## Help
## ./push.sh <your-api-key>
##

dotnet nuget push ./bin/Release/ -k $1 -s https://api.nuget.org/v3/index.json --skip-duplicate