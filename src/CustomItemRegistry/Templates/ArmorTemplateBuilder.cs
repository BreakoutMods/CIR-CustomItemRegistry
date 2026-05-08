namespace ValheimCustomItemRegistry
{
    /// <summary>
    /// Template builder for armor slots.
    /// </summary>
    public sealed class ArmorTemplateBuilder : TemplateBuilderBase<ArmorTemplateBuilder>
    {
        internal ArmorTemplateBuilder(CustomItemDefinition definition, string templateName, ItemDrop.ItemData.ItemType itemType)
            : base(definition, templateName)
        {
            definition.ItemType = itemType;
            definition.StackSize = definition.StackSize ?? 1;
            definition.MaxQuality = definition.MaxQuality ?? 4;
            definition.MaxDurability = definition.MaxDurability ?? 1000f;
            definition.DurabilityPerLevel = definition.DurabilityPerLevel ?? 200f;
            definition.CanBeRepaired = definition.CanBeRepaired ?? true;
            definition.TemplateRequiresArmor = true;
        }

        public ArmorTemplateBuilder Armor(float value, float perLevel = 0f)
        {
            Definition.Armor = value;
            Definition.ArmorPerLevel = perLevel;
            return this;
        }

        public ArmorTemplateBuilder Durability(float value, float perLevel = 0f) => DurabilityValues(value, perLevel);
        public ArmorTemplateBuilder MaxQuality(int value) => MaxQualityValue(value);
        public ArmorTemplateBuilder Repairable(bool value = true) => RepairableValue(value);
    }
}
