namespace ValheimCustomItemRegistry
{
    /// <summary>
    /// Template builder for shields.
    /// </summary>
    public sealed class ShieldTemplateBuilder : TemplateBuilderBase<ShieldTemplateBuilder>
    {
        internal ShieldTemplateBuilder(CustomItemDefinition definition)
            : base(definition, "Shield")
        {
            definition.ItemType = ItemDrop.ItemData.ItemType.Shield;
            definition.StackSize = definition.StackSize ?? 1;
            definition.MaxQuality = definition.MaxQuality ?? 4;
            definition.MaxDurability = definition.MaxDurability ?? 200f;
            definition.DurabilityPerLevel = definition.DurabilityPerLevel ?? 50f;
            definition.CanBeRepaired = definition.CanBeRepaired ?? true;
            definition.TemplateRequiresBlockPower = true;
        }

        public ShieldTemplateBuilder Block(float power, float force = 0f, float parry = 1.5f)
        {
            Definition.BlockPower = power;
            Definition.DeflectionForce = force;
            Definition.SharedDataConfigurators.Add(shared => shared.m_timedBlockBonus = parry);
            return this;
        }

        public ShieldTemplateBuilder Durability(float value, float perLevel = 0f) => DurabilityValues(value, perLevel);
        public ShieldTemplateBuilder MaxQuality(int value) => MaxQualityValue(value);
        public ShieldTemplateBuilder Repairable(bool value = true) => RepairableValue(value);
    }
}
