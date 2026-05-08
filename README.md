# BreakoutMods CIR - Custom Item Registry

Part of the BreakoutMods modding suite.

CIR, short for Custom Item Registry, is a Valheim modding API for developers who want to ship custom 3D item prefabs from AssetBundles without rewriting ObjectDB, ZNetScene, recipe, and multiplayer registration glue in every mod.

Repository: [BreakoutMods/CIR-CustomItemRegistry](https://github.com/BreakoutMods/CIR-CustomItemRegistry)

The library is built for BepInEx 5.x, Harmony, and Jotunn. It leans on Jotunn's `PrefabManager` and `ItemManager` for multiplayer-safe prefab and recipe registration, while exposing both the original small API and a CIR 0.2 builder API.

## Usage

Reference `CustomItemRegistry.dll` from your mod project and add BepInEx dependencies:

```csharp
[BepInDependency(CustomItemRegistryPlugin.PluginGuid)]
[BepInDependency(Jotunn.Main.ModGuid)]
```

Register a modern item from `Awake`:

```csharp
CustomItemRegistry.Item("BreakoutSword")
    .FromBundle(assetBundlePath, "BreakoutSwordPrefab")
    .DisplayName("$item_breakoutsword")
    .Description("$item_breakoutsword_desc")
    .Icon("BreakoutSwordIcon")
    .Recipe(recipe => recipe
        .At("forge")
        .RepairAt("forge")
        .StationLevel(2)
        .Amount(1)
        .Requires("FineWood", 4)
        .Requires("Iron", 10))
    .Gear(gear => gear
        .OneHandedWeapon()
        .Weight(2f)
        .StackSize(1)
        .Durability(200f)
        .MaxQuality(4)
        .SlashDamage(35f)
        .BlockPower(20f)
        .MovementModifier(-0.05f))
    .Register();
```

The original API remains supported:

```csharp
CustomItemRegistry.RegisterItem(
    "MyCrystalSword",
    Path.Combine(Path.GetDirectoryName(Info.Location), "myitems"),
    "MyCrystalSwordPrefab",
    new CraftingRecipe(
        new List<Ingredient>
        {
            new Ingredient("FineWood", 8),
            new Ingredient("Crystal", 12),
            new Ingredient("Silver", 4)
        },
        "forge",
        1));
```

The AssetBundle prefab must include an `ItemDrop` component. If it does not already include a `ZNetView`, CIR adds one and marks it persistent for networked item drops.

## Features

#### API

- `CustomItemRegistry.Item(string itemName)` fluent builder entrypoint.
- `RegisterItem(string itemName, string assetBundlePath, string prefabName, CraftingRecipe recipe)` legacy API.
- `RegisterItem(CustomItemDefinition definition)`, `TryRegisterItem(...)`, and `RegisterItems(...)`.
- `CustomItemBuilder`, `RecipeBuilder`, `GearBuilder`, `CustomItemDefinition`, `ItemRegistrationResult`, and `CustomItemRegistrationException`.
- `CraftingRecipe` with ingredients, crafting station, repair station, station level, amount, enabled flag, require-only-one ingredient, and quality result multiplier.

#### Item Metadata

- Display name, description, icon sprite by AssetBundle asset name, or direct `Sprite`.
- Weight, stack size, durability, max quality, tool tier, teleportable flag, and repairable flag.
- Item type helpers for one-handed weapon, two-handed weapon, shield, bow, tool, armor, helmet, chest, legs, and utility.
- Armor, block power, block force, movement modifier, base damages, and per-level damages.
- `.ConfigureSharedData(...)` escape hatch for advanced `ItemDrop.ItemData.SharedData` edits.

#### Registration

- Loads AssetBundles from absolute paths or paths relative to `BepInEx/plugins`.
- Loads and clones the requested prefab, renaming the clone to the public item name.
- Builds Jotunn item and recipe data from CIR definitions.
- Registers prefabs through Jotunn's `PrefabManager` for multiplayer-safe ZNetScene registration.
- Registers items and recipes through Jotunn's `ItemManager`.
- Includes Harmony timing patches on `ObjectDB.CopyOtherDB` and `ZNetScene.Awake` to flush items into live databases when Valheim creates or copies them.
- Validates missing bundle paths, missing prefab assets, missing `ItemDrop`, duplicate item names, invalid recipes, and missing craftable item icons with clearer log messages.

#### Developer Example

The `src/ExampleCustomItemPlugin` project shows builder, definition, try-register, and legacy API usage. Its sample AssetBundle and prefab names are placeholders, so replace them with real assets before shipping.

## Installation

_If you're using a mod manager, you can likely ignore this section._

1. Install BepInEx 5.x for Valheim.
2. Install Jotunn.
3. Copy `CustomItemRegistry.dll` into `BepInEx/plugins/CustomItemRegistry`.
4. Copy developer mods that depend on the API into their own folder under `BepInEx/plugins`.
5. Put each developer mod's AssetBundles next to that developer mod, or pass an absolute AssetBundle path to CIR.

## Building

This repo expects to live under a Valheim install like:

```text
Valheim dedicated server/
  BepInEx/
  valheim_server_Data/
  Modding/
    CIR-CustomItemRegistry/
```

Build with:

```powershell
.\build.ps1 -Configuration Release
```

Debug builds copy the API DLL into `BepInEx/plugins/CustomItemRegistry` and the example DLL into `BepInEx/plugins/ExampleCustomItemPlugin`.

## Notes For Asset Authors

- Use internal Valheim prefab names for ingredients, such as `Wood`, `Bronze`, `LeatherScraps`, `FineWood`, or `Crystal`.
- Use Jotunn's accepted crafting station names. Common examples are `piece_workbench`, `forge`, and `piece_cauldron`. Passing `null` or an empty string makes the recipe craftable without a station.
- Include an item icon in the `ItemDrop` shared data, pass a direct `Sprite`, or call `.Icon("SpriteAssetName")` for craftable items.
- Keep prefab names stable once released. Renaming a registered item prefab can affect existing saves and inventories.

## Bugs, Support, Contributions

Please open issues with the Valheim version, BepInEx version, Jotunn version, the item prefab name, and the relevant BepInEx log lines. Pull requests that keep the public API small and improve interop with Jotunn are welcome.

## Changelog

See `CHANGELOG.md`.
