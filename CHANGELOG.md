# Changelog

## 0.6.1

- Fixed `NullReferenceException` in `ItemDrop.Awake()` that occurred when registering item prefabs whose `ItemDrop` component had uninitialized `m_itemData` or `m_itemData.m_shared`. CIR now initializes those fields on the source prefab before calling `Object.Instantiate()`, preventing the crash and the downstream `SetupVisEquipment` NRE when the item was later equipped.

## 0.6.0

- Added optional YAML and JSON item-pack loading through soft runtime detection of `YamlDotNet` and `Newtonsoft.Json`.
- Added `LoadItemPacks`, `LoadItemPacksFromDirectory`, `LoadItemPack`, and `GetItemPackParserStatus` APIs.
- Added item-pack result and diagnostic types for per-file and per-item load errors.
- Added recursive auto-load from `BepInEx/config/CustomItemRegistry/packs`.
- Added raw-definition item-pack docs and examples.

## 0.5.0

- Added strict prefab validation that lists missing `ItemDrop`, `Rigidbody`, `ZNetView`, `ZSyncTransform`, and collider components.
- Added opt-in safe prefab preparation that can create missing `ItemDrop`, `Rigidbody`, `ZNetView`, and `ZSyncTransform` components.
- Added `PrefabPreparationBuilder` controls for ItemDrop auto-add, physics auto-add, collider warnings, and Texture2D icon fallback.
- Added opt-in Texture2D-to-Sprite fallback for `.Icon("AssetName")`.
- Added strict wearable armor visual validation for `Chest`, `Legs`, `Helmet`, and `Shoulder` items to catch missing `SkinnedMeshRenderer`, mesh, root bone, or bone references before equip-time crashes.
- Improved missing prefab and icon AssetBundle diagnostics with case-sensitivity hints and candidate asset names.
- Added grouped registration logging with item type, icon source, recipe station, added components, and warning count.
- Added prefab preparation validation harness cases for simple scriptless prefabs.

## 0.4.0

- Added typed recipe helpers with `VanillaItem`, `CraftingStation`, and `ItemRef`.
- Added recipe builder overloads for vanilla items, crafting station enums, raw prefab refs, and soft third-party mod item refs.
- Added ingredient factory helpers for typed vanilla and item reference requirements.
- Added clearer validation for invalid helper enum values and empty item references.
- Added optional dependency diagnostics for missing third-party recipe ingredients.
- Added recipe helper docs and updated the example plugin with vanilla and modded ingredient examples.

## 0.3.0

- Added typed item templates for common Valheim item types.
- Added weapon, shield, armor, bow, ammo, tool, food, and material template builders.
- Added template-aware validation for missing weapon damage, shield block power, armor value, and food stats.
- Updated the example plugin to compile-test every template entrypoint.
- Added a template validation harness for core failure/default scenarios.
- Added template documentation.

## 0.2.0

- Added fluent `CustomItemRegistry.Item(...)` builder API.
- Added `CustomItemDefinition`, `ItemRegistrationResult`, and `CustomItemRegistrationException`.
- Added definition, batch, and try-register overloads.
- Added common item metadata, icon loading, practical gear stats, and shared-data escape hatch support.
- Added preloaded and embedded AssetBundle builder sources inspired by production item-pack mods.
- Added ammo/material/consumable/trinket item type helpers, parry, attack force, attack tuning, projectile tuning, and damage modifier builder methods.
- Fixed recipe validation to allow upgrade-only ingredients where craft amount is `0` and amount per level is greater than `0`.
- Extended `CraftingRecipe` with repair station, station level, enabled flag, require-only-one ingredient, and quality result multiplier fields.
- Improved validation and registration logging.
- Updated the example plugin with builder, definition, try-register, and legacy API examples.

## 0.1.0

- Initial public API for registering custom 3D item prefabs from AssetBundles.
- Added Jotunn-backed ObjectDB and ZNetScene registration.
- Added example plugin for API consumers.
