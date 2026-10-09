# MODLOG — Hell's City

## 2026-10-09 — Initial source scaffold
- Host selected: ULTRAKILL; Unity; Melty-provided BepInEx 5.
- Companion design: Limbus Company content must be read from the player's own install at runtime. Per supplied Melty brief, Limbus is not in Melty's catalog and cannot currently be a Melty host/companion; do not falsely tag it as a required companion. Listing role must follow Melty's actual recipe rules, pending tool access.
- Project folder inspection: no existing project or game install found in this workspace.
- Melty MCP connection attempt failed before HTTP negotiation because the environment could not resolve `melty.gg`; no Melty tools were available in the current tool list.
- universal-modder was reviewed via its public skill page. Its recommended loop is inspect the real install, select the game's supported loader, build one verified slice, test in the actual game, then package.
- Created BepInEx plugin scaffold, data sheets, and player-facing README. The plugin currently adds a diagnostic overlay and Limbus path/asset-candidate inventory only.
- No copyrighted game assets or third-party binaries included.

## Blocking unknowns
1. Actual ULTRAKILL install and BepInEx/Unity assembly versions.
2. Actual Limbus install layout and the current content formats/compatibility of the assets intended for reuse.
3. ULTRAKILL custom level construction/spawn hooks for this exact game version.
4. Melty `game_info`, `search_mashups`, recipe, package, and one-click validation results.
5. Listing credits, license, and remix permissions.
6. Compile, live gameplay test, and genuine screenshot.

## Test status
- JSON parsing/schema consistency: pending.
- C# compile: not run; game and BepInEx assemblies are absent.
- In-game test: not run; game is absent.
- Melty validation/upload: not run; API DNS resolution failed.
