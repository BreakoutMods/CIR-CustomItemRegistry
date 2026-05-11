using System.Collections.Generic;

namespace ValheimCustomItemRegistry
{
    internal sealed class ItemPackDto
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public List<ItemPackItemDto> Items { get; set; }
    }

    internal sealed class ItemPackItemDto
    {
        public string ItemName { get; set; }
        public string AssetBundle { get; set; }
        public string AssetBundlePath { get; set; }
        public string PrefabName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string ItemType { get; set; }
        public float? Weight { get; set; }
        public int? StackSize { get; set; }
        public bool? Teleportable { get; set; }
        public float? Durability { get; set; }
        public float? DurabilityPerLevel { get; set; }
        public int? MaxQuality { get; set; }
        public int? ToolTier { get; set; }
        public float? Armor { get; set; }
        public float? ArmorPerLevel { get; set; }
        public float? MovementModifier { get; set; }
        public ItemPackDamageDto Damages { get; set; }
        public ItemPackDamageDto DamagesPerLevel { get; set; }
        public ItemPackRecipeDto Recipe { get; set; }
        public ItemPackPrefabPreparationDto PrefabPreparation { get; set; }
    }

    internal sealed class ItemPackDamageDto
    {
        public float? Blunt { get; set; }
        public float? Slash { get; set; }
        public float? Pierce { get; set; }
        public float? Fire { get; set; }
        public float? Frost { get; set; }
        public float? Lightning { get; set; }
        public float? Poison { get; set; }
        public float? Spirit { get; set; }
        public float? Chop { get; set; }
        public float? Pickaxe { get; set; }
    }

    internal sealed class ItemPackRecipeDto
    {
        public bool? Enabled { get; set; }
        public string CraftingStation { get; set; }
        public string RepairStation { get; set; }
        public int? MinStationLevel { get; set; }
        public int? Amount { get; set; }
        public bool? RequireOnlyOneIngredient { get; set; }
        public int? QualityResultAmountMultiplier { get; set; }
        public List<ItemPackIngredientDto> Ingredients { get; set; }
    }

    internal sealed class ItemPackIngredientDto
    {
        public string Item { get; set; }
        public int? Amount { get; set; }
        public int? AmountPerLevel { get; set; }
        public bool? Recover { get; set; }
        public string SourceModGuid { get; set; }
    }

    internal sealed class ItemPackPrefabPreparationDto
    {
        public bool? AutoAddItemDrop { get; set; }
        public bool? AutoAddPhysics { get; set; }
        public bool? WarnOnMissingCollider { get; set; }
        public bool? AllowTextureIconFallback { get; set; }
        public bool? ValidateWearableVisuals { get; set; }
    }
}
