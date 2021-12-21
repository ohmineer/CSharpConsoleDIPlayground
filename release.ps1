[CmdletBinding(DefaultParameterSetName = "set1")]
param(
  [switch]$Help,
  [switch]$PushRemote,
  [Parameter()][parameter(ParameterSetName = 'set1')][ValidateSet('Major', 'Minor')][string]$VersionIncrement = 'Minor',
  [Parameter()][parameter(ParameterSetName = 'set2')][string]$NextVersion
)

Set-StrictMode -Version 2
$ErrorActionPreference = 'Stop'

if ($Help.IsPresent) {
  Get-Help $PSCommandPath
  exit 0
}

if (-not ([string]::IsNullOrWhiteSpace($NextVersion))) {
  $versionPattern = "^\d+\.\d+$"
  $matchResult = $NextVersion -match $versionPattern
  if ($matchResult) {
    $version = nbgv prepare-release --nextVersion $NextVersion --format json | ConvertFrom-Json
  }
  else {
    Write-Error "Version must be formatted as Major.Minor"
    exit 0
  }
}
else {
  $version = nbgv prepare-release --versionIncrement $VersionIncrement --format json | ConvertFrom-Json
}

# Prepare the release
# Create a tag for the new branch in both local repo and origin.
$tag = nbgv tag $version.NewBranch.Commit
# Push everything to remote if it exists
if ((git ls-remote --exit-code origin) -and ($PushRemote.IsPresent)) {
  git push origin

  $tagPattern = "v\d+(?:\.\d+)+"
  $matchResult = $tag -match $tagPattern
  if ($matchResult) {
    git push origin matches[0]
  }
}
