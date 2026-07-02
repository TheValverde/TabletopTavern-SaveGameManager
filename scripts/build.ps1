#Requires -Version 5.1
param(
    [string]$Configuration = "Release",
    [switch]$Deploy
)

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path $PSScriptRoot -Parent
$project = Join-Path $repoRoot "src\TTMod.SaveGameManager\TTMod.SaveGameManager.csproj"

$buildArgs = @("build", $project, "-c", $Configuration, "--nologo", "-v", "q")
if (-not $Deploy) {
    $buildArgs += "/p:SkipGameDeploy=true"
}

& dotnet @buildArgs
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

$dll = Join-Path $repoRoot "src\TTMod.SaveGameManager\bin\$Configuration\net472\TTMod.SaveGameManager.dll"
Write-Host "Built: $dll" -ForegroundColor Green
