# Hugo's Save Game Manager — Nexus upload fields

Paste BBCode from [description.bbcode.txt](description.bbcode.txt) into the Nexus editor (BBCode mode).

| Field | Value |
|-------|-------|
| Title | `Hugo's Tabletop Tavern - Save Game Manager` |
| Category | Gameplay |
| Summary | Main menu Load Run and New Run buttons with a saved run list and per-run retire. |
| Short description | Replaces Continue/New Campaign with Load Run and New Run. Load Run lists saved runs with details and Retire Run on each row. Requires BepInEx Core and Modding Core. |
| Tags | `save`, `campaign`, `main menu`, `load`, `bepinex`, `gameplay`, `single player`, `quality of life` |
| File | `Hugos-TabletopTavern-Save-Game-Manager-0.1.1.zip` |
| Requirements | `Tabletop Tavern - BepInEx Core`, `Tabletop Tavern - Modding Core` |
| Optional | `Tabletop Tavern - Configuration Manager`, `Hugo's Endless Run` (loop count on run rows) |

Build the zip:

```powershell
cd github\TabletopTavern-SaveGameManager
.\scripts\build-release.ps1
```

Output: `dist/Hugos-TabletopTavern-Save-Game-Manager-0.1.1.zip`
