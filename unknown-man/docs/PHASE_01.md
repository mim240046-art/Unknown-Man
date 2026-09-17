# Phase 1 — Architecture Foundation

## Purpose
Freeze production contracts before Minecraft-specific behavior is added. The shared content is authoritative; Java and Bedrock implement equivalent runtime ports.

## Deliverables
- Namespaced, versioned content IDs.
- JSON Schema documents for evidence, anomalies, puzzles, configuration, and saves.
- Cross-content semantic validator.
- Java state/event/save contracts that compile without Minecraft dependencies.
- Bedrock TypeScript state/event/save contracts that type-check without beta APIs.
- Release gate policy and compatibility ledger.

## Dependency policy
Minecraft imports are forbidden in the shared contract layer. Platform adapters may import only APIs recorded in `docs/COMPATIBILITY_LEDGER.md`.

## Test gate
`npm run check` must be green. Java contracts must compile with the baseline JDK before the Fabric adapter is introduced.
