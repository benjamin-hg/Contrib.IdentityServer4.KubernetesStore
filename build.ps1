Param ([string]$Version = "0.1-pre")
$ErrorActionPreference = "Stop"
pushd $(Split-Path -Path $MyInvocation.MyCommand.Definition -Parent)

src\build.ps1 $Version
src\test.ps1

popd
