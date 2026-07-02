#Requires -Version 5.1
param(
    [string]$Version = "",
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"
. "$PSScriptRoot\_release-common.ps1"

$repoRoot = Get-TtRepoRoot
$project = Join-Path $repoRoot "src\TTMod.SaveGameManager\TTMod.SaveGameManager.csproj"

if (-not $Version) {
    [xml]$props = Get-Content $project
    $Version = $props.Project.PropertyGroup.Version.Trim()
}

$packFolderName = "Hugos-TabletopTavern-Save-Game-Manager-$Version"
$stageRoot = Get-TtStagingDir -PackageFolderName $packFolderName -RepoRoot $repoRoot
$outZip = Join-Path (Get-TtDistDir -RepoRoot $repoRoot) "$packFolderName.zip"

Write-Host "=== Hugo's Save Game Manager release ===" -ForegroundColor Cyan
Write-Host "Version: $Version"
Write-Host ""

& "$PSScriptRoot\build.ps1" -Configuration $Configuration
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

$pluginDll = Join-Path $repoRoot "src\TTMod.SaveGameManager\bin\$Configuration\net472\TTMod.SaveGameManager.dll"
if (-not (Test-Path $pluginDll)) {
    throw "Plugin DLL missing: $pluginDll"
}

Reset-TtStagingDir -StageRoot $stageRoot

$pluginsDir = Ensure-TtPluginsDir -StageRoot $stageRoot
Copy-Item -Path $pluginDll -Destination (Join-Path $pluginsDir "TTMod.SaveGameManager.dll") -Force
Copy-Item (Join-Path $repoRoot "docs\nexus\INSTALL.txt") (Join-Path $stageRoot "INSTALL.txt") -Force
Copy-Item (Join-Path $repoRoot "changelogs\CHANGELOG.md") (Join-Path $stageRoot "CHANGELOG.md") -Force

Compress-TtStage -StageRoot $stageRoot -OutZip $outZip

Write-Host ""
Write-Host "Built:" -ForegroundColor Green
Write-Host "  $outZip"
