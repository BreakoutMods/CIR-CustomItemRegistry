# CIR Item Templates

CIR templates are typed shortcuts over the normal `CustomItemDefinition` pipeline. They set common Valheim defaults and validate the fields that are usually required for that item kind.

## Template Map

| Template | Valheim item type | Defaults | Required |
| --- | --- | --- | --- |
| `.AsSword(...)` | `OneHandedWeapon` | stack 1, quality 4, durable, repairable | damage |
| `.AsAxe(...)` | `OneHandedWeapon` | stack 1, quality 4, durable, repairable | damage |
| `.AsMace(...)` | `OneHandedWeapon` | stack 1, quality 4, durable, repairable | damage |
| `.AsSpear(...)` | `OneHandedWeapon` | stack 1, quality 4, durable, repairable | damage |
| `.AsKnife(...)` | `OneHandedWeapon` | stack 1, quality 4, durable, repairable | damage |
| `.AsAtgeir(...)` | `TwoHandedWeapon` | stack 1, quality 4, durable, repairable | damage |
| `.AsBow(...)` | `Bow` | stack 1, quality 4, durable, repairable | pierce damage |
| `.AsArrow(...)` | `Ammo` | stack 100, quality 1, non-durable, non-repairable | damage |
| `.AsShield(...)` | `Shield` | stack 1, quality 4, durable, repairable | block power |
| `.AsArmorChest(...)` | `Chest` | stack 1, quality 4, durable, repairable | armor |
| `.AsArmorLegs(...)` | `Legs` | stack 1, quality 4, durable, repairable | armor |
| `.AsHelmet(...)` | `Helmet` | stack 1, quality 4, durable, repairable | armor |
| `.AsCape(...)` | `Shoulder` | stack 1, quality 4, durable, repairable | armor |
| `.AsTool(...)` | `Tool` | stack 1, quality 4, durable, repairable | none |
| `.AsFood(...)` | `Consumable` | stack 10, quality 1, non-durable, non-repairable | health, stamina, or eitr |
| `.AsMaterial(...)` | `Material` | stack 50, quality 1, non-durable, non-repairable | none |

## Examples

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
    .Recipe(recipe => recipe.At("forge").StationLevel(2)
        .Requires("Iron", 18)
        .Requires("FineWood", 4)
        .Requires("LeatherScraps", 2)
        .Requires("Iron", 0, 8))
    .Register();
```

```csharp
CustomItemRegistry.Item("BM_TowerShield")
    .FromBundle(assetBundlePath, "BM_TowerShieldPrefab")
    .Icon("BM_TowerShieldIcon")
    .AsShield(shield => shield
        .Block(power: 60f, force: 40f, parry: 1.5f)
        .Movement(-0.15f))
    .Register();
```

```csharp
CustomItemRegistry.Item("BM_HunterJerkin")
    .FromBundle(assetBundlePath, "BM_HunterJerkinPrefab")
    .Icon("BM_HunterJerkinIcon")
    .AsArmorChest(armor => armor
        .Armor(18f, perLevel: 2f)
        .Movement(-0.02f))
    .Register();
```

```csharp
CustomItemRegistry.Item("BM_HoneyStew")
    .FromBundle(assetBundlePath, "BM_HoneyStewPrefab")
    .Icon("BM_HoneyStewIcon")
    .AsFood(food => food
        .Stats(health: 40f, stamina: 25f, eitr: 0f)
        .Duration(1200f)
        .Regen(2f))
    .Register();
```

```csharp
CustomItemRegistry.Item("BM_RawGem")
    .FromBundle(assetBundlePath, "BM_RawGemPrefab")
    .Icon("BM_RawGemIcon")
    .AsMaterial(material => material
        .StackSize(50)
        .Weight(0.2f)
        .Value(25))
    .Register();
```

## Escape Hatches

Templates do not replace the raw builders. You can still call `.Gear(...)` and `.ConfigureSharedData(...)` after a template when an item needs fields CIR does not model yet.
