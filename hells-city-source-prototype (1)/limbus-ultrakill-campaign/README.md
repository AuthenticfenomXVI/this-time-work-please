# Hell's City — Limbus Company × ULTRAKILL

**Status: source prototype, not yet a playable campaign.** This repository contains the first BepInEx 5 plugin scaffold, campaign data sheets, and a runtime discovery diagnostic. It does not contain game assets, and it does not yet add mission geometry, enemy prefabs, boss logic, or combat encounters.

## Intended experience

A solo, combat-first ULTRAKILL campaign moving through several recognizable City districts. V1 encounters Limbus Company Sinners, Abnormalities, and major rivals/bosses, while environmental storytelling keeps the plot light.

## Requirements and host

- **Host:** ULTRAKILL (Steam installation; Melty's catalog slug is `ultrakill`).
- **Loader:** BepInEx 5, installed by Melty for ULTRAKILL.
- **Companion content:** Limbus Company is not currently in Melty's catalog. The prototype searches the player's local Steam libraries at runtime and inventories likely asset files without copying them. The final implementation must load only from the player's own Limbus installation and must show a clear in-game message if it is missing.
- **Solo:** no multiplayer behavior is planned.

## What works in this prototype

- BepInEx plugin entry point and an F8 diagnostic overlay.
- Reads mission names from `Data/areas.json` for the prototype overlay when the sheet is packaged beside the plugin.
- Attempts to locate Limbus Company in common Steam library paths, including extra libraries listed in `libraryfolders.vdf`.
- Reports likely asset-bundle/data candidates by filename/extension; it does **not** extract or redistribute assets.
- Lists the planned campaign areas and encounters in the overlay.

## What is not implemented yet

- Real Limbus asset decoding/loading. The game install and its current asset layout must be inspected first; a filename scan is not proof an asset can be loaded into ULTRAKILL's Unity runtime.
- Custom City level scenes, navigation, enemy spawning, Sinner behavior, Abnormality behavior, boss phases, checkpoints, and completion triggers.
- A compiled DLL, a real in-game test, gameplay capture, Melty recipe validation, upload, or publication.

## Build notes

The plugin references the **local** ULTRAKILL/BepInEx assemblies. Do not commit or distribute those assemblies. On a Windows development machine with ULTRAKILL and BepInEx 5 installed, open `src/HellsCity.csproj` and set `ULTRAKILL_DIR` to the game's root directory, then build for `netstandard2.0`. If the installed game's assemblies require a different target framework or reference set, confirm that from the actual install before changing the project.

The current sandbox has no game installation, no project-specific assemblies, and cannot resolve Melty's API host, so this scaffold has not been compiled or run in ULTRAKILL. Do not treat it as a release-ready mod.

## Asset and licensing policy

No ULTRAKILL or Limbus Company models, textures, audio, levels, executable files, decompiled source, or other game assets are included. The final mod should read the player's own installed content at runtime, only after compatibility and permitted access are established. Credits and remix permissions remain **unconfirmed**; do not publish until the listing's credit/license/remix fields are explicitly set.
