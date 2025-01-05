#!/bin/bash
## Help
# Run ./pack.sh <version-suffix>
##
dotnet pack -c Release --version-suffix $1