# UNKNOWN MAN

Production source repository for a seven-day Minecraft horror-survival experience.

## Frozen baselines

- Java Edition 26.2: Fabric Loader 0.19.3, Loom 1.17, Gradle 9.5.1, JDK 25, Mojang official names.
- Bedrock Edition 1.26.40: manifest format 2, stable `@minecraft/server` 2.9.0.

## Current production gate

Phase 1 establishes versioned contracts, shared content schemas, validation, platform-neutral state definitions, and build checks. It is not a playable release by itself. A player-ready release is permitted only after all 20 gates in `docs/RELEASE_GATES.md` pass.

## Requirements

- Node.js 20 or newer
- npm
- JDK 25 with `javac` available on `PATH`

## Install and verify

The following commands work in Windows PowerShell/Command Prompt, macOS, and Linux because platform-specific file discovery is handled by Node.js:

```text
npm install
npm run check
```

Individual checks:

```text
npm run validate
npm test
npm run check:bedrock
npm run check:java
```

`check:java` recursively discovers Java source files without Bash `find` or command substitution. It compiles them into `builds/java-contracts` and returns a non-zero status if `javac` is unavailable or compilation fails.

No experimental Minecraft API is authorized in the release branch.
