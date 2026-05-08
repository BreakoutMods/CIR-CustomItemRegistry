using System;
using System.Collections.Generic;
using UnityEngine;

namespace ValheimCustomItemRegistry
{
    /// <summary>
    /// Complete code-first definition for a custom item registration.
    /// </summary>
    public sealed class CustomItemDefinition
    {
        public string ItemName { get; set; }
        public string AssetBundlePath { get; set; }
        public AssetBundle AssetBundle { get; set; }
        public string PrefabName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string IconAssetName { get; set; }
        public Sprite Icon { get; set; }
        public CraftingRecipe Recipe { get; set; }
        public bool HasRecipe { get; set; }

        public ItemDrop.ItemData.ItemType? ItemType { get; set; }
        public float? Weight { get; set; }
        public int? StackSize { get; set; }
        public float? MaxDurability { get; set; }
        public float? DurabilityPerLevel { get; set; }
        public int? MaxQuality { get; set; }
        public int? ToolTier { get; set; }
        public float? Armor { get; set; }
        public float? ArmorPerLevel { get; set; }
        public float? BlockPower { get; set; }
        public float? BlockPowerPerLevel { get; set; }
        public float? DeflectionForce { get; set; }
        public float? DeflectionForcePerLevel { get; set; }
        public float? MovementModifier { get; set; }
        public bool? Teleportable { get; set; }
        public bool? CanBeRepaired { get; set; }

        public HitData.DamageTypes Damages;
        public bool HasDamages { get; set; }
        public HitData.DamageTypes DamagesPerLevel;
        public bool HasDamagesPerLevel { get; set; }

        public IList<Action<ItemDrop.ItemData.SharedData>> SharedDataConfigurators { get; private set; }

        internal string TemplateName { get; set; }
        internal bool TemplateRequiresDamage { get; set; }
        internal bool TemplateRequiresBlockPower { get; set; }
        internal bool TemplateRequiresArmor { get; set; }
        internal bool TemplateRequiresFoodStats { get; set; }
        internal bool TemplateHasFoodStats { get; set; }

        public CustomItemDefinition()
        {
            SharedDataConfigurators = new List<Action<ItemDrop.ItemData.SharedData>>();
        }

        public CustomItemDefinition(string itemName)
            : this()
        {
            ItemName = itemName;
        }
    }
}
