# Changelog

## 0.1.1

- Soft compatibility with **Endless Campaign**: loop count shown on each run row when that mod is loaded (reads `com.tabletop-tavern.endless-campaign-profile.json`, no hard dependency)

## 0.1.0

- Replace the single Continue/New Campaign button with **Load Run** and **New Run**
- **New Run** always opens the hero and difficulty setup panel (existing new campaign flow)
- **Load Run** opens a scrollable list of saved runs with hero, book, layer, chapter, difficulty, seed, and gold details
- Each run row includes an embedded **Retire Run** action
- Starting a New Run while a campaign save exists parks the current run under `TTModRuns/` so you can load it later
- Hide the vanilla Abandon Run button (retire lives on each list row instead)
