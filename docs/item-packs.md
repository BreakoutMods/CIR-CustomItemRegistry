# CIR Item Packs

CIR 0.6 can load custom item definitions from YAML or JSON files. This is optional: CIR still works without YamlDotNet or Json.NET installed.

## Dependencies

- YAML: install `ValheimModding-YamlDotNet`.
- JSON: install `ValheimModding-JsonDotNET`.
- CIR detects loaded assemblies named `YamlDotNet` and `Newtonsoft.Json`.
- CIR does not bundle either library.

## Folders

CIR auto-loads packs from:

```text
BepInEx/config/CustomItemRegistry/packs
```

Mods can load their own pack folder:

```csharp
CustomItemRegistry.LoadItemPacksFromDirectory(
    Path.Combine(Path.GetDirectoryName(Info.Location), "packs"));
```

AssetBundle paths resolve in this order:

1. absolute path
2. relative to the item-pack file
3. relative to `BepInEx/plugins`

## YAML Example

```yaml
name: Breakout Sample Pack
version: 1.0.0
items:
  - itemName: BM_SampleMaterial
    assetBundle: assets/bm_items
    prefabName: BM_SampleMaterial
    displayName: $item_bm_samplematerial
    description: A sample custom material.
    icon: BM_SampleMaterialIcon
    itemType: Material
    weight: 1
    stackSize: 50
    prefabPreparation:
      autoAddItemDrop: true
      autoAddPhysics: true
      warnOnMissingCollider: true
      allowTextureIconFallback: true

  - itemName: BM_SampleSword
    assetBundle: assets/bm_items
    prefabName: BM_SampleSword
    displayName: $item_bm_samplesword
    description: A sample custom sword.
    icon: BM_SampleSwordIcon
    itemType: OneHandedWeapon
    weight: 2
    stackSize: 1
    durability: 200
    durabilityPerLevel: 50
    maxQuality: 4
    movementModifier: -0.05
    damages:
      slash: 42
      pierce: 6
    damagesPerLevel:
      slash: 5
    recipe:
      craftingStation: Forge
      repairStation: Forge
      minStationLevel: 2
      amount: 1
      ingredients:
        - item: Iron
          amount: 12
        - item: FineWood
          amount: 4
        - item: MagicCore
          amount: 1
          sourceModGuid: com.otherauthor.valheim.magicmod
```

## JSON Example

```json
{
  "name": "Breakout JSON Sample Pack",
  "version": "1.0.0",
  "items": [
    {
      "itemName": "BM_SampleArmorChest",
      "assetBundle": "assets/bm_items",
      "prefabName": "BM_SampleArmorChest",
      "displayName": "$item_bm_samplearmor",
      "description": "A sample custom armor chest.",
      "icon": "BM_SampleArmorIcon",
      "itemType": "Chest",
      "weight": 10,
      "stackSize": 1,
      "durability": 1000,
      "durabilityPerLevel": 200,
      "maxQuality": 4,
      "armor": 24,
      "armorPerLevel": 3,
      "recipe": {
        "craftingStation": "Forge",
        "repairStation": "Forge",
        "minStationLevel": 1,
        "amount": 1,
        "ingredients": [
          { "item": "Iron", "amount": 20 }
        ]
      }
    }
  ]
}
```

## Field Reference

Pack fields:

- `name`
- `version`
- `items`

Item fields:

- `itemName`
- `assetBundle`
- `prefabName`
- `displayName`
- `description`
- `icon`
- `itemType`
- `weight`
- `stackSize`
- `teleportable`
- `durability`
- `durabilityPerLevel`
- `maxQuality`
- `toolTier`
- `armor`
- `armorPerLevel`
- `movementModifier`
- `damages`
- `damagesPerLevel`
- `recipe`
- `prefabPreparation`

Damage fields:

- `blunt`
- `slash`
- `pierce`
- `fire`
- `frost`
- `lightning`
- `poison`
- `spirit`
- `chop`
- `pickaxe`

Recipe fields:

- `enabled`
- `craftingStation`
- `repairStation`
- `minStationLevel`
- `amount`
- `requireOnlyOneIngredient`
- `qualityResultAmountMultiplier`
- `ingredients`

Ingredient fields:

- `item`
- `amount`
- `amountPerLevel`
- `recover`
- `sourceModGuid`

Prefab preparation fields:

- `autoAddItemDrop`
- `autoAddPhysics`
- `warnOnMissingCollider`
- `allowTextureIconFallback`
- `validateWearableVisuals`

## Notes

- `craftingStation` and `repairStation` accept CIR station names such as `Forge` or raw Valheim prefab strings such as `forge`.
- `itemType` accepts Valheim item type names such as `Material`, `OneHandedWeapon`, `Chest`, `Legs`, `Helmet`, and `Shoulder`.
- YAML and JSON packs use the same schema.
- Bad files and bad items are reported per file/item so one failure does not block later files.
- Item packs do not sync over multiplayer yet. Server and client must still install matching packs manually.
