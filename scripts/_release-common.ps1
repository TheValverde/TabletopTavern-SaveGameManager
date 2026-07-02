#Requires -Version 5.1

function Get-TtRepoRoot {
    return Split-Path $PSScriptRoot -Parent
}

function Get-TtDistDir {
    param([string]$RepoRoot = (Get-TtRepoRoot))
    return Join-Path $RepoRoot "dist"
}

function Get-TtStagingDir {
    param(
        [string]$PackageFolderName,
        [string]$RepoRoot = (Get-TtRepoRoot)
    )

    return Join-Path (Get-TtDistDir -RepoRoot $RepoRoot) "staging\$PackageFolderName"
}

function Reset-TtStagingDir {
    param([string]$StageRoot)

    if (Test-Path $StageRoot) {
        Remove-Item $StageRoot -Recurse -Force
    }

    New-Item -ItemType Directory -Force -Path $StageRoot | Out-Null
}

function Compress-TtStage {
    param(
        [string]$StageRoot,
        [string]$OutZip
    )

    $distDir = Split-Path $OutZip -Parent
    New-Item -ItemType Directory -Force -Path $distDir | Out-Null

    if (Test-Path $OutZip) {
        Remove-Item $OutZip -Force
    }

    Push-Location $StageRoot
    try {
        Compress-Archive -Path * -DestinationPath $OutZip -Force
    } finally {
        Pop-Location
    }
}

function Ensure-TtPluginsDir {
    param([string]$StageRoot)

    $pluginsDir = Join-Path $StageRoot "BepInEx\plugins"
    New-Item -ItemType Directory -Force -Path $pluginsDir | Out-Null
    return $pluginsDir
}
