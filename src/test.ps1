Param ([string]$Version = "0.1-debug")
$ErrorActionPreference = "Stop"
pushd $(Split-Path -Path $MyInvocation.MyCommand.Definition -Parent)

dotnet test --no-build --configuration Release UnitTests\UnitTests.csproj

popd
