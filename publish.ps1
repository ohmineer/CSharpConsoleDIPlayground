<#
.SYNOPSIS
Publishes the app
.DESCRIPTION
This script cleans all files in the output folder from a configuration given as parameter.
Then, it publishes the project according to user preferences.
.PARAMETER SelfContained
Flag to include framework in the publish directory. Users won't need to install framework.
.PARAMETER SingleFile
Flag to produce one executable which bundles all libraries.
.PARAMETER Configuration
Build configuration. Debug is the default value.
Only Debug or Release values are allowed.
.PARAMETER Target
Target architecture of published app. 'win-x64' is the default value.
Only win-x64, linux-x64 and osx-x64 are allowed.
.EXAMPLE
publish.ps1
.EXAMPLE
publish.ps1 -Configuration Release -SelfContained -SingleFile
#>

[CmdletBinding()]
param(
  [switch]$Help,
  [switch]$SelfContained,
  [switch]$SingleFile,
  [Parameter()][ValidateSet('Debug', 'Release')][string]$Configuration = 'Debug',
  [Parameter()][ValidateSet('win-x64', 'linux-x64', 'osx-x64')][string]$Target = 'win-x64'
)

Set-StrictMode -Version 2
$ErrorActionPreference = 'Stop'
$DirectoryPath = "**/bin/$($Configuration)/**/$($Target)"
$IsSingleFile = $false
$IsSelfContained = $false

if ($Help) {
  Get-Help $PSCommandPath
  exit 0
}

if ($SelfContained) {
  $IsSelfContained = $true
}

if (($SingleFile) -and ($IsSelfContained)) {
  $IsSingleFile = $true
}

dotnet clean -c $Configuration --nologo

# Clean any additional folder created during build or run processes.
Get-ChildItem $DirectoryPath -Recurse | ForEach-Object {
  Remove-Item $_.FullName -Force -Recurse
}

dotnet publish -c $Configuration --nologo `
  -r $Target --self-contained $IsSelfContained `
  -p:PublishSingleFile=$IsSingleFile `
  -p:PublicRelease=true

Invoke-Item "$($DirectoryPath)/publish"
