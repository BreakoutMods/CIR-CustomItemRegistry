# BreakoutMods CIR - Custom Item Registry

Part of the BreakoutMods modding suite.

CIR, short for Custom Item Registry, is a Valheim modding API for developers who want to ship custom 3D item prefabs from AssetBundles without rewriting ObjectDB, ZNetScene, recipe, and multiplayer registration glue in every mod.

Repository: [BreakoutMods/CIR-CustomItemRegistry](https://github.com/BreakoutMods/CIR-CustomItemRegistry)

The library is built for BepInEx 5.x, Harmony, and Jotunn. It leans on Jotunn's `PrefabManager` and `ItemManager` for multiplayer-safe prefab and recipe registration, while exposing the original small API, the CIR 0.2 raw builder API, and CIR 0.3 typed item templates.

## Usage

Reference `CustomItemRegistry.dll` from your mod project and add BepInEx dependencies:

```csharp
[BepInDependency(CustomItemRegistryPlugin.PluginGuid)]
[BepInDependency(Jotunn.Main.ModGuid)]
```

Register a templated item from `Awake`:

```csharp
CustomItemRegistry.Item("BM_IronLongsword")
    .FromEmbeddedResource("MyMod.Assets.items", typeof(MyPlugin).Assembly, "BM_IronLongsword")
    .DisplayName("$item_bm_ironlongsword")
    .Description("$item_bm_ironlongsword_desc")
    .Icon("BM_IronLongswordIcon")
    .AsSword(sword => sword
        .Slash(48f, perLevel: 6f)
        .Block(24f, force: 18f, parry: 2f)
        .Durability(250f, perLevel: 50f)
        .Attack(stamina: 14f, force: 35f)
        .Movement(-0.05f))
    .Recipe(recipe => recipe
        .At("forge")
        .StationLevel(2)
        .Requires("FineWood", 4)
        .Requires("Iron", 10)
        .Requires("Iron", 0, 8))
    .Register();
```

You can still use the raw 0.2 builder when you need direct shared-data control:

```csharp
CustomItemRegistry.Item("BreakoutMaterial")
    .FromBundle(assetBundlePath, "BreakoutMaterialPrefab")
    .Icon("BreakoutMaterialIcon")
    .Gear(gear => gear.Material().StackSize(50))
    .ConfigureSharedData(shared => shared.m_value = 25)
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
- CIR 0.3 template builders: `WeaponTemplateBuilder`, `ShieldTemplateBuilder`, `ArmorTemplateBuilder`, `BowTemplateBuilder`, `AmmoTemplateBuilder`, `ToolTemplateBuilder`, `FoodTemplateBuilder`, and `MaterialTemplateBuilder`.
- `CraftingRecipe` with ingredients, crafting station, repair station, station level, amount, enabled flag, require-only-one ingredient, and quality result multiplier.
- AssetBundles can be loaded from file paths, passed as preloaded `AssetBundle` instances, or loaded from embedded resources with `.FromEmbeddedResource(...)`.

#### Item Templates

- `.AsSword(...)`, `.AsAxe(...)`, `.AsMace(...)`, `.AsSpear(...)`, `.AsKnife(...)`, and `.AsAtgeir(...)` for melee weapons.
- `.AsBow(...)` and `.AsArrow(...)` for ranged weapons and ammo.
- `.AsShield(...)`, `.AsArmorChest(...)`, `.AsArmorLegs(...)`, `.AsHelmet(...)`, and `.AsCape(...)` for defense items.
- `.AsTool(...)`, `.AsFood(...)`, and `.AsMaterial(...)` for common non-weapon items.
- Template-aware validation catches missing weapon damage, shield block power, armor value, and food stats before asset loading.

#### Item Metadata

- Display name, description, icon sprite by AssetBundle asset name, or direct `Sprite`.
- Weight, stack size, durability, max quality, tool tier, teleportable flag, and repairable flag.
- Item type helpers for weapons, shields, bows, ammo, tools, armor slots, materials, consumables, shoulder items, trinkets, torches, and utility items.
- Armor, block power, block force, parry, attack force, movement modifier, base damages, per-level damages, and damage modifiers.
- Primary and secondary attack tuning for stamina, eitr, health costs, force multipliers, projectile count, projectile velocity, draw duration, draw stamina drain, reload time, and reload stamina drain.
- `.ConfigureSharedData(...)` escape hatch for advanced `ItemDrop.ItemData.SharedData` edits.

#### Registration

- Loads AssetBundles from absolute paths or paths relative to `BepInEx/plugins`.
- Loads and clones the requested prefab, renaming the clone to the public item name.
- Builds Jotunn item and recipe data from CIR definitions.
- Registers prefabs through Jotunn's `PrefabManager` for multiplayer-safe ZNetScene registration.
- Registers items and recipes through Jotunn's `ItemManager`.
- Includes Harmony timing patches on `ObjectDB.CopyOtherDB` and `ZNetScene.Awake` to flush items into live databases when Valheim creates or copies them.
- Validates missing bundle paths, missing prefab assets, missing `ItemDrop`, duplicate item names, invalid recipes, and missing craftable item icons with clearer log messages.
- Validates template-specific required fields with the template name in the error message.

#### Developer Example

The `src/ExampleCustomItemPlugin` project shows template, raw builder, definition, try-register, validation harness, and legacy API usage. Its sample AssetBundle and prefab names are placeholders, so replace them with real assets before shipping.

## Project Layout

```text
CIR-CustomItemRegistry/
  CIR-CustomItemRegistry.sln
  build.ps1
  docs/
    templates.md
  src/
    CustomItemRegistry/
      API/          Public API contracts and registration facade
      Builders/     Fluent item, recipe, and gear builders
      Templates/    Typed Valheim item template builders
      Patches/      Harmony timing patches
      Plugin/       BepInEx plugin entrypoint
    ExampleCustomItemPlugin/
      Examples/     Developer-facing usage examples
      Testing/      Lightweight compile/validation harnesses
```

The public namespace and assembly identity stay stable even though the source files are grouped by responsibility.

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
- Upgrade-only recipe requirements are valid. Use `.Requires("Bronze", 0, 4)` when an ingredient should only be consumed by upgrades.
- Self-contained mods can embed an AssetBundle in the DLL and call `.FromEmbeddedResource("Namespace.BundleName", typeof(MyPlugin).Assembly, "PrefabName")`.
- Use templates for normal items first. Drop to `.Gear(...)` or `.ConfigureSharedData(...)` only for unusual behavior.
- Keep prefab names stable once released. Renaming a registered item prefab can affect existing saves and inventories.

## Bugs, Support, Contributions

Please open issues with the Valheim version, BepInEx version, Jotunn version, the item prefab name, and the relevant BepInEx log lines. Pull requests that keep the public API small and improve interop with Jotunn are welcome.

## Changelog

See `CHANGELOG.md`.
