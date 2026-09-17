# UNKNOWN MAN — Milestone 1 Vertical Slice

Target: Unity 6.3 LTS, editor patch 6000.3.22f1, URP 17.3, Input System 1.16.

This project is a real Unity project foundation for the pasted Milestone 1 specification. It is deliberately asset-light: the village, props, lights, HUD, mobile controls, player, interactions, save system and graphics scaling are created from C# so the project stays compact and replaceable.

## What is included

- URP project foundation with automatic editor setup.
- Procedural abandoned village: road, houses, shop, school, storage, forest, well and final gate.
- First-person CharacterController player.
- PC keyboard/mouse/controller input.
- Mobile touch joysticks and action buttons.
- Door interaction and evidence/diary interaction.
- Basic inventory/evidence journal data model.
- Versioned JSON save data in Application.persistentDataPath.
- Graphics Auto / Very Low / Low / Medium / High / Ultra presets.
- Core render scale, texture limit, shadow, LOD, fog and view-complexity scaling.
- Optional developer performance overlay (F8 in editor/development builds).
- Minimal horror atmosphere: fog, night lighting, distant silhouette, environmental mystery.
- No sexual, religious, self-harm, alcohol, gambling or gore-heavy content.

## Controls

PC:
- WASD move
- Mouse look
- Shift sprint
- C crouch
- E interact
- I inventory panel
- J journal panel
- F3 graphics/settings panel
- G cycle graphics preset
- Esc pause / unlock cursor
- F8 performance overlay

Mobile:
- Left joystick: movement
- Right joystick: look
- On-screen action buttons: interact, sprint, crouch, inventory, journal, pause

## Open and run

1. Install Unity 6.3 LTS, preferably 6000.3.22f1 or a later 6.3 LTS patch.
2. Open this folder through Unity Hub.
3. Let Package Manager resolve URP 17.3 and Input System 1.16.
4. Unity runs `Unknown Man/Setup Project Foundation` automatically through the editor initialization script. If needed, run that menu command once.
5. Open `Assets/Scenes/Milestone1.unity` and press Play.

## Build targets

Android and Windows are intended targets. Add the corresponding Unity Hub build support modules, then use Unity's Build Profiles/Build Settings to create the Android and Windows builds from `Assets/Scenes/Milestone1.unity`.

## Deliberate milestone boundary

This is Milestone 1. The seven-day campaign, advanced Unknown Man AI, seven forms, anomaly framework, puzzle framework, survival/cooking/sleep systems, cinematics and multiple endings are intentionally not included yet because the source specification explicitly calls for incremental construction.

## Verification note

The environment used to generate this archive does not contain a Unity Editor binary, so I could not execute a Unity import, compile, or device build inside this session. The project files are organized for Unity 6.3 LTS and the implementation uses APIs documented for this Unity generation; final compilation/import/device-build verification must be done by opening the project in Unity.

## Cloud Android APK build (no Unity on laptop)

A GitHub Actions workflow is included at `.github/workflows/android-apk.yml`. It uses GameCI's current Unity Builder v4, targets Android, and exports an APK artifact.

See `GITHUB_APK_BUILD.md` for the short setup steps. Cloud builds still require a Unity license configured in GitHub Actions secrets.
