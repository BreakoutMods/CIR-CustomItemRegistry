# BreakoutMods CIR - Custom Item Registry

Part of the BreakoutMods modding suite.

CIR, short for Custom Item Registry, is a Valheim modding API for developers who want to ship custom 3D item prefabs from AssetBundles without rewriting ObjectDB, ZNetScene, recipe, and multiplayer registration glue in every mod.

Repository: [BreakoutMods/CIR-CustomItemRegistry](https://github.com/BreakoutMods/CIR-CustomItemRegistry)

Community: [BreakoutMods Discord](https://discord.gg/ArmCF3nscW)

Support development: [BreakoutMods Patreon](https://www.patreon.com/breakoutmods)

The library is built for BepInEx 5.x, Harmony, and Jotunn. It leans on Jotunn's `PrefabManager` and `ItemManager` for multiplayer-safe prefab and recipe registration, while exposing the original small API, the CIR 0.2 raw builder API, CIR 0.3 typed item templates, and CIR 0.4 typed recipe helpers.

## Usage

Reference `CustomItemRegistry.dll` from your mod project and add BepInEx dependencies:

```csharp
[BepInDependency(CustomItemRegistryPlugin.PluginGuid)]
[BepInDependency(Jotunn.Main.ModGuid)]
```

Valheim also has a game type named `CraftingStation`. If your project references Valheim assemblies, add an alias for CIR's enum:

```csharp
using CIRCraftingStation = ValheimCustomItemRegistry.CraftingStation;
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
        .At(CIRCraftingStation.Forge)
        .StationLevel(2)
        .Requires(VanillaItem.FineWood, 4)
        .Requires(VanillaItem.Iron, 10)
        .Requires(VanillaItem.Iron, 0, 8))
    .Register();
```

Typed helpers remove most recipe string memorization, but raw prefab strings remain supported:

```csharp
using static ValheimCustomItemRegistry.ItemRefs;

CustomItemRegistry.Item("BM_MagicBlade")
    .FromBundle(assetBundlePath, "BM_MagicBladePrefab")
        .AsSword(sword => sword.Slash(40f).Spirit(10f))
    .Recipe(recipe => recipe
        .At(CIRCraftingStation.Forge)
        .Requires(VanillaItem.Silver, 10)
        .Requires(Modded("com.otherauthor.valheim.magicmod", "MagicCore"), 1))
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

By default, CIR expects your AssetBundle prefab to already contain the required Valheim item components: `ItemDrop`, `Rigidbody`, `ZNetView`, `ZSyncTransform`, and a collider. If any are missing, CIR refuses registration and lists the missing components. For simple test items, you can opt into CIR auto-preparation with `.PrefabPreparation(...)`.

## Features

#### API

- `CustomItemRegistry.Item(string itemName)` fluent builder entrypoint.
- `RegisterItem(string itemName, string assetBundlePath, string prefabName, CraftingRecipe recipe)` legacy API.
- `RegisterItem(CustomItemDefinition definition)`, `TryRegisterItem(...)`, and `RegisterItems(...)`.
- `CustomItemBuilder`, `RecipeBuilder`, `GearBuilder`, `CustomItemDefinition`, `ItemRegistrationResult`, and `CustomItemRegistrationException`.
- CIR 0.3 template builders: `WeaponTemplateBuilder`, `ShieldTemplateBuilder`, `ArmorTemplateBuilder`, `BowTemplateBuilder`, `AmmoTemplateBuilder`, `ToolTemplateBuilder`, `FoodTemplateBuilder`, and `MaterialTemplateBuilder`.
- CIR 0.4 recipe helpers: `VanillaItem`, `CraftingStation`, `ItemRef`, `ItemRefs`, and `ToPrefabName()` extension methods.
- `CraftingRecipe` with ingredients, crafting station, repair station, station level, amount, enabled flag, require-only-one ingredient, and quality result multiplier.
- AssetBundles can be loaded from file paths, passed as preloaded `AssetBundle` instances, or loaded from embedded resources with `.FromEmbeddedResource(...)`.
- `CustomItemBuilder.PrefabPreparation(...)` opts into safe auto-preparation for simple prefabs or adjusts strict validation behavior.
- Wearable armor validation checks `Chest`, `Legs`, `Helmet`, and `Shoulder` prefabs for skinned mesh setup before registration, catching common `SetupVisEquipment` equip crashes early.

#### Typed Recipe Helpers

- `RecipeBuilder.At(CIRCraftingStation.Forge)` and `RepairAt(CIRCraftingStation.Workbench)` map known stations to Jotunn prefab names.
- `RecipeBuilder.Requires(VanillaItem.Iron, 10)` maps common Valheim ingredients to prefab names.
- `RecipeBuilder.Requires(ItemRef.Modded("com.author.mod", "MagicCore"), 1)` soft-links third-party mod items without compile-time references.
- `ItemRef.Prefab("SomePrefab")` and raw `.Requires("SomePrefab", 1)` remain available for custom or newly added prefabs.

#### Item Templates

- `.AsSword(...)`, `.AsAxe(...)`, `.AsMace(...)`, `.AsSpear(...)`, `.AsKnife(...)`, and `.AsAtgeir(...)` for melee weapons.
- `.AsBow(...)` and `.AsArrow(...)` for ranged weapons and ammo.
- `.AsShield(...)`, `.AsArmorChest(...)`, `.AsArmorLegs(...)`, `.AsHelmet(...)`, and `.AsCape(...)` for defense items.
- `.AsTool(...)`, `.AsFood(...)`, and `.AsMaterial(...)` for common non-weapon items.
- Template-aware validation catches missing weapon damage, shield block power, armor value, and food stats before asset loading.

#### Item Metadata

- Display name, description, icon sprite by AssetBundle asset name, or direct `Sprite`.
- Icon asset names load `Sprite` by default. Opt-in prefab preparation can allow CIR to convert a `Texture2D` with the same name into a runtime sprite.
- Weight, stack size, durability, max quality, tool tier, teleportable flag, and repairable flag.
- Item type helpers for weapons, shields, bows, ammo, tools, armor slots, materials, consumables, shoulder items, trinkets, torches, and utility items.
- Armor, block power, block force, parry, attack force, movement modifier, base damages, per-level damages, and damage modifiers.
- Primary and secondary attack tuning for stamina, eitr, health costs, force multipliers, projectile count, projectile velocity, draw duration, draw stamina drain, reload time, and reload stamina drain.
- `.ConfigureSharedData(...)` escape hatch for advanced `ItemDrop.ItemData.SharedData` edits.

#### Registration

- Loads AssetBundles from absolute paths or paths relative to `BepInEx/plugins`.
- Loads and clones the requested prefab, renaming the clone to the public item name.
- Strictly validates missing `ItemDrop`, `Rigidbody`, `ZNetView`, `ZSyncTransform`, and collider components by default.
- Can prepare simple prefabs by adding missing `ItemDrop`, `Rigidbody`, `ZNetView`, and `ZSyncTransform` when explicitly enabled.
- Warns when an auto-prepared prefab has no collider, but does not add a guessed collider.
- Builds Jotunn item and recipe data from CIR definitions.
- Registers prefabs through Jotunn's `PrefabManager` for multiplayer-safe ZNetScene registration.
- Registers items and recipes through Jotunn's `ItemManager`.
- Includes Harmony timing patches on `ObjectDB.CopyOtherDB` and `ZNetScene.Awake` to flush items into live databases when Valheim creates or copies them.
- Validates missing bundle paths, missing prefab assets, duplicate item names, invalid recipes, and missing craftable item icons with clearer log messages and AssetBundle candidate names.
- Validates template-specific required fields with the template name in the error message.

## Minimal Unity Prefab

For production items, build the Unity prefab like a Valheim item and include `ItemDrop`, `Rigidbody`, `ZNetView`, `ZSyncTransform`, and a collider.

For quick test items, the Unity prefab can be very small if you explicitly opt into auto-preparation:

```text
AssetBundle
  MyItemPrefab
    visible mesh/model
    collider recommended
  MyItemIcon Sprite or Texture2D
```

CIR will add the common Valheim item scripts at registration time only when `.PrefabPreparation(...)` enables it. Use templates such as `.AsMaterial(...)`, `.AsArmorChest(...)`, or `.AsSword(...)` so CIR knows which item type and stats to apply.

CIR does not repair Unity `Missing Script` references, does not choose collider shape/size, and does not guess vanilla asset references. If you use vanilla assets, use Jotunn `JVLmock_...` references.

Wearable armor is stricter than normal items. `.AsArmorChest(...)`, `.AsArmorLegs(...)`, `.AsHelmet(...)`, and `.AsCape(...)` need a real Valheim-style wearable visual in the prefab: at least one `SkinnedMeshRenderer` with a mesh, `rootBone`, and valid bones. Chest, legs, and cape prefabs should usually follow the vanilla `attach_skin...` hierarchy. A static mesh plus icon is fine for materials, but not for equipped armor.

If imported armor equips with broken visuals or null references even though the skinned renderer exists, base the prefab on a vanilla armor item and consider calling Jotunn `BoneReorder.ApplyOnEquipmentChanged()` from your plugin.

Advanced mods with their own custom equipment visual pipeline can disable only this check with `.PrefabPreparation(prep => prep.ValidateWearableVisuals(false))`.

Example opt-in for a simple mesh prefab:

```csharp
CustomItemRegistry.Item("SimpleItem")
    .FromBundle(assetBundlePath, "SimpleItemPrefab")
    .PrefabPreparation(prep => prep
        .AutoAddItemDrop()
        .AutoAddPhysics()
        .WarnOnMissingCollider()
        .AllowTextureIconFallback())
    .AsMaterial()
    .Register();
```

#### Developer Example

The `src/ExampleCustomItemPlugin` project shows template, raw builder, definition, try-register, validation harness, and legacy API usage. Its sample AssetBundle and prefab names are placeholders, so replace them with real assets before shipping.

## Project Layout

```text
CIR-CustomItemRegistry/
  CIR-CustomItemRegistry.sln
  build.ps1
  docs/
    recipes.md
    templates.md
    roadmap/
      developer-helper-api.md
  src/
    CustomItemRegistry/
      API/          Public API contracts and registration facade
      Builders/     Fluent item, recipe, and gear builders
      Helpers/      Vanilla item, station, and modded item refs
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

- Prefer `VanillaItem` and `CraftingStation` for common ingredients and stations. Use raw strings for uncommon or newly added prefabs.
- Use `ItemRef.Modded("other.mod.guid", "PrefabName")` when depending on a third-party item. CIR logs that source mod GUID if the prefab is missing.
- Use Jotunn's accepted crafting station names when you pass raw strings. Common examples are `piece_workbench`, `forge`, and `piece_cauldron`. Passing `null` or an empty string makes the recipe craftable without a station.
- Include an item icon in the `ItemDrop` shared data, pass a direct `Sprite`, or call `.Icon("SpriteAssetName")` for craftable items.
- `.Icon("AssetName")` can also use a `Texture2D` with the same name if no `Sprite` exists when `AllowTextureIconFallback()` is enabled.
- Add colliders in Unity for reliable drop, pickup, and physics behavior. Strict mode requires one; auto-preparation mode can warn instead.
- Upgrade-only recipe requirements are valid. Use `.Requires("Bronze", 0, 4)` when an ingredient should only be consumed by upgrades.
- Self-contained mods can embed an AssetBundle in the DLL and call `.FromEmbeddedResource("Namespace.BundleName", typeof(MyPlugin).Assembly, "PrefabName")`.
- Use templates for normal items first. Drop to `.Gear(...)` or `.ConfigureSharedData(...)` only for unusual behavior.
- Keep prefab names stable once released. Renaming a registered item prefab can affect existing saves and inventories.

## Bugs, Support, Contributions

Please open issues with the Valheim version, BepInEx version, Jotunn version, the item prefab name, and the relevant BepInEx log lines. Pull requests that keep the public API small and improve interop with Jotunn are welcome.

## Changelog

See `CHANGELOG.md`.
