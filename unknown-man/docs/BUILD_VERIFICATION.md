# Phase 1 build verification — revision 0.1.1

Verified in the build sandbox on 2026-09-16:

- Shared production content validation: PASS
- Semantic and cross-platform build-tool tests: 5/5 PASS
- Bedrock TypeScript contracts: PASS
- TypeScript dependency: pinned to 5.9.3 and represented in `package-lock.json`
- Windows-safe URL conversion: `fileURLToPath` used for fixture paths
- Cross-platform Java source discovery: PASS
- Java compiler invocation: included in `npm run check`, uses `spawnSync` with `shell: false`
- Java source compilation: BLOCKED in this sandbox because JDK 25/`javac` is unavailable; the check correctly returns a non-zero error

The sandbox could resolve lockfile metadata but could not download the TypeScript tarball during a later clean install because outbound npm DNS became unavailable. Existing TypeScript compilation verified the source contracts. A clean `npm install` remains part of CI on a network-enabled build machine.

This remains a production source milestone, not a player release.
