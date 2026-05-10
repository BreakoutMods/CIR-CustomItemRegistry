# CIR Recipe Helpers

CIR 0.4 adds typed helpers for common Valheim ingredients, crafting stations, and soft references to third-party mod items. These helpers convert to the same prefab-name strings used by the original recipe API, so existing mods can mix typed and raw string recipes freely.

## Vanilla Ingredients

Use `VanillaItem` when CIR already knows the prefab name:

```csharp
using CIRCraftingStation = ValheimCustomItemRegistry.CraftingStation;

.Recipe(recipe => recipe
    .At(CIRCraftingStation.Forge)
    .Requires(VanillaItem.Bronze, 12)
    .Requires(VanillaItem.FineWood, 4)
    .Requires(VanillaItem.LeatherScraps, 2)
    .Requires(VanillaItem.Bronze, 0, 8))
```

`VanillaItem.ToPrefabName()` returns the internal prefab name for direct interop:

```csharp
string prefabName = VanillaItem.Iron.ToPrefabName(); // Iron
```

## Crafting Stations

Valheim also has a game type named `CraftingStation`, so examples use `CIRCraftingStation` as an alias for `ValheimCustomItemRegistry.CraftingStation`.

`CIRCraftingStation.None` maps to `null`, which makes a recipe craftable without a station.

| Helper | Prefab name |
| --- | --- |
| `CIRCraftingStation.None` | `null` |
| `CIRCraftingStation.Workbench` | `piece_workbench` |
| `CIRCraftingStation.Forge` | `forge` |
| `CIRCraftingStation.Stonecutter` | `piece_stonecutter` |
| `CIRCraftingStation.Cauldron` | `piece_cauldron` |
| `CIRCraftingStation.ArtisanTable` | `piece_artisanstation` |
| `CIRCraftingStation.BlackForge` | `blackforge` |
| `CIRCraftingStation.GaldrTable` | `piece_magetable` |
| `CIRCraftingStation.EitrRefinery` | `piece_eitrrefinery` |

## Third-Party Ingredients

Use `ItemRef.Modded` when another mod owns the ingredient prefab. CIR does not hard-reference that mod; it keeps Jotunn's normal late reference behavior and adds clearer warnings if the prefab is missing when databases are live.

```csharp
.Recipe(recipe => recipe
    .At(CIRCraftingStation.Forge)
    .Requires(VanillaItem.Silver, 10)
    .Requires(ItemRef.Modded("com.otherauthor.valheim.magicmod", "MagicCore"), 1))
```

For shorter examples, import:

```csharp
using static ValheimCustomItemRegistry.ItemRefs;
```

Then call:

```csharp
.Requires(Modded("com.otherauthor.valheim.magicmod", "MagicCore"), 1)
```

## Raw Strings Still Work

Use raw strings when a prefab is not in `VanillaItem` yet, or when you intentionally want to depend on a known prefab name without adding source mod metadata:

```csharp
.Requires("SomeNewPrefabName", 1)
.Requires(ItemRef.Prefab("SomeNewPrefabName"), 1)
```

Use `ItemRef.FromRegisteredCIRItem("MyOtherCIRItem")` when a recipe depends on another item registered by CIR in the same mod suite.
