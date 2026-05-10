# CIR 0.4 Developer Helper API

Implemented in CIR 0.4.

This roadmap item added typed recipe helpers for common Valheim ingredient prefabs, crafting stations, and soft references to third-party modded items. The implementation keeps the original string-based recipe API intact and converts helper values back to prefab names before Jotunn registration.

## Delivered

- `VanillaItem` and `CraftingStation` enums.
- `ToPrefabName()` extension methods.
- `ItemRef` and `ItemRefs` factory helpers.
- Recipe builder overloads for typed stations and ingredients.
- Ingredient factory helpers for vanilla and item references.
- Validation for invalid enum values and empty item refs.
- Clearer warnings for missing third-party recipe ingredients.
- README, recipe docs, example plugin usage, and validation harness coverage.
