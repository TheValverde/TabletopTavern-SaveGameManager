# Save Game Manager

Overhauls the main menu campaign button into **Load Run** and **New Run**, with a scrollable run list and per-run **Retire Run** actions.

Private gameplay mod for Tabletop Tavern. Separate from the [Tabletop Tavern Modding](https://github.com/TheValverde/TabletopTavern-Modding) foundation.

## Requirements

1. Tabletop Tavern - BepInEx Core
2. Tabletop Tavern - Modding Core
3. Tabletop Tavern - Configuration Manager (recommended)

## Plugin identity

| Field | Value |
|-------|-------|
| GUID | `com.tabletop-tavern.save-game-manager` |
| DLL | `TTMod.SaveGameManager.dll` |

## Build

```powershell
$env:TT_GAME_ROOT = "D:\Overflow Programs\Steam\steamapps\common\TabletopTavern"
.\scripts\build.ps1 -Deploy
```

## Behavior

- **New Run** always opens the vanilla new campaign setup panel. If a campaign save exists, it is parked under `TTModRuns/` first.
- **Load Run** opens a panel listing active and parked runs. Click a row to continue; **Retire Run** deletes that run.
- The vanilla **Abandon Run** button is hidden; retire lives on each list row.

## Endless Campaign compatibility

No hard dependency. When Endless Campaign is loaded, each run row also shows **Loops** from `BepInEx/config/com.tabletop-tavern.endless-campaign-profile.json`.

## Test

1. Launch the game and open the main menu
2. Confirm **New Run** and **Load Run** appear (Load Run directly under New Run)
3. Start a run, exit to menu, open **Load Run**, and confirm run details appear
4. Retire a run and confirm it disappears from the list
5. With Endless Campaign installed, confirm loop count appears on run rows

## Release zip

```powershell
.\scripts\build-release.ps1
```

Output: `dist/Hugos-TabletopTavern-Save-Game-Manager-<version>.zip`
