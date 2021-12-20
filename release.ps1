[CmdletBinding()]
param(
  [switch]$Help,
  [Parameter()][ValidateSet('Major', 'Minor')][string]$VersionIncrement = 'Minor'
)

Set-StrictMode -Version 2
$ErrorActionPreference = 'Stop'

if ($Help) {
  Get-Help $PSCommandPath
  exit 0
}

# Pattern of version produced by prepare
$tagPattern = "v\d+(?:\.\d+)+"

# Prepare the release
$version = nbgv prepare-release --versionIncrement $VersionIncrement --format json | ConvertFrom-Json
# Create a tag for the new branch in both local repo and origin.
$tag = nbgv tag $version.NewBranch.Commit
# Push everything to remote if it exists
if (git ls-remote --exit-code origin) {
  git push origin

  $matchResult = $tag -match $tagPattern
  if ($matchResult) {
    git push origin matches[0]
  }
}
