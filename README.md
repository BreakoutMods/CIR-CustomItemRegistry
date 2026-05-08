# BreakoutMods CIR - Custom Item Registry

Part of the BreakoutMods modding suite.

CIR, short for Custom Item Registry, is a small Valheim modding API for developers who want to ship custom 3D item prefabs from AssetBundles without rewriting the same ObjectDB, ZNetScene, and multiplayer registration glue in every mod.

Repository: [BreakoutMods/CIR-CustomItemRegistry](https://github.com/BreakoutMods/CIR-CustomItemRegistry)

The library is built for BepInEx 5.x, Harmony, and Jotunn. It leans on Jotunn's `PrefabManager` and `ItemManager` for multiplayer-safe prefab and recipe registration, while exposing one focused API for other plugins:

```csharp
CustomItemRegistry.RegisterItem(string itemName, string assetBundlePath, string prefabName, CraftingRecipe recipe);
```

## Usage

Reference `CustomItemRegistry.dll` from your mod project and add a BepInEx dependency:

```csharp
[BepInDependency(CustomItemRegistryPlugin.PluginGuid)]
[BepInDependency(Jotunn.Main.ModGuid)]
```

Register your item from `Awake`:

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

The AssetBundle prefab must include an `ItemDrop` component. If it does not already include a `ZNetView`, the registry adds one and marks it persistent for networked item drops.

## Installation

_If you're using a mod manager, you can likely ignore this section._

1. Install BepInEx 5.x for Valheim.
2. Install Jotunn.
3. Copy `CustomItemRegistry.dll` into `BepInEx/plugins/CustomItemRegistry`.
4. Copy developer mods that depend on the API into their own folder under `BepInEx/plugins`.
5. Put each developer mod's AssetBundles next to that developer mod, or pass an absolute AssetBundle path to `RegisterItem`.

## Features

#### API

- `CustomItemRegistry` public API class.
- `RegisterItem(string itemName, string assetBundlePath, string prefabName, CraftingRecipe recipe)`.
- `CraftingRecipe` struct with `List<Ingredient> ingredients`, `string craftingStation`, and `int amount`.
- `Ingredient` struct for item prefab name, amount, and optional upgrade amount per level.

#### Registration

- Loads AssetBundles from absolute paths or paths relative to `BepInEx/plugins`.
- Loads and clones the requested prefab, renaming the clone to the public item name.
- Builds Jotunn recipe data from the custom `CraftingRecipe`.
- Registers prefabs through Jotunn's `PrefabManager` for multiplayer-safe ZNetScene registration.
- Registers items and recipes through Jotunn's `ItemManager`.
- Includes Harmony timing patches on `ObjectDB.CopyOtherDB` and `ZNetScene.Awake` to flush items into live databases when Valheim creates or copies them.

#### Developer Example

The `src/ExampleCustomItemPlugin` project shows how another mod can depend on this API and register a custom item. Its sample AssetBundle name and prefab name are placeholders, so replace them with your real bundle and prefab before shipping.

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
- Include an item icon in the `ItemDrop` shared data if the item is craftable. Jotunn validates craftable custom items and expects an icon.

## Bugs, Support, Contributions

Please open issues with the Valheim version, BepInEx version, Jotunn version, the item prefab name, and the relevant BepInEx log lines. Pull requests that keep the public API small and improve interop with Jotunn are welcome.

## Changelog

See `CHANGELOG.md`.
