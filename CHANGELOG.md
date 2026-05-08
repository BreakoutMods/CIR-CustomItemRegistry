# Changelog

## 0.2.1

- Added preloaded and embedded AssetBundle builder sources inspired by production item-pack mods.
- Added ammo/material/consumable/trinket item type helpers, parry, attack force, attack tuning, projectile tuning, and damage modifier builder methods.
- Fixed recipe validation to allow upgrade-only ingredients where craft amount is `0` and amount per level is greater than `0`.

## 0.2.0

- Added fluent `CustomItemRegistry.Item(...)` builder API.
- Added `CustomItemDefinition`, `ItemRegistrationResult`, and `CustomItemRegistrationException`.
- Added definition, batch, and try-register overloads.
- Added common item metadata, icon loading, practical gear stats, and shared-data escape hatch support.
- Extended `CraftingRecipe` with repair station, station level, enabled flag, require-only-one ingredient, and quality result multiplier fields.
- Improved validation and registration logging.
- Updated the example plugin with builder, definition, try-register, and legacy API examples.

## 0.1.0

- Initial public API for registering custom 3D item prefabs from AssetBundles.
- Added Jotunn-backed ObjectDB and ZNetScene registration.
- Added example plugin for API consumers.
