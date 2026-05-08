namespace ValheimCustomItemRegistry
{
    /// <summary>
    /// Template builder for tools.
    /// </summary>
    public sealed class ToolTemplateBuilder : TemplateBuilderBase<ToolTemplateBuilder>
    {
        internal ToolTemplateBuilder(CustomItemDefinition definition)
            : base(definition, "Tool")
        {
            definition.ItemType = ItemDrop.ItemData.ItemType.Tool;
            definition.StackSize = definition.StackSize ?? 1;
            definition.MaxQuality = definition.MaxQuality ?? 4;
            definition.MaxDurability = definition.MaxDurability ?? 200f;
            definition.DurabilityPerLevel = definition.DurabilityPerLevel ?? 50f;
            definition.CanBeRepaired = definition.CanBeRepaired ?? true;
        }

        public ToolTemplateBuilder Chop(float value, float perLevel = 0f)
        {
            Damage(d => { d.m_chop = value; return d; });
            if (perLevel != 0f)
            {
                DamagePerLevel(d => { d.m_chop = perLevel; return d; });
            }

            return this;
        }

        public ToolTemplateBuilder Pickaxe(float value, float perLevel = 0f)
        {
            Damage(d => { d.m_pickaxe = value; return d; });
            if (perLevel != 0f)
            {
                DamagePerLevel(d => { d.m_pickaxe = perLevel; return d; });
            }

            return this;
        }

        public ToolTemplateBuilder ToolTier(int value)
        {
            Definition.ToolTier = value;
            return this;
        }

        public ToolTemplateBuilder Attack(float stamina, float force = 0f)
        {
            Definition.SharedDataConfigurators.Add(shared => shared.m_attackForce = force);
            PrimaryAttack(attack => attack.m_attackStamina = stamina);
            return this;
        }

        public ToolTemplateBuilder Durability(float value, float perLevel = 0f) => DurabilityValues(value, perLevel);
        public ToolTemplateBuilder MaxQuality(int value) => MaxQualityValue(value);
        public ToolTemplateBuilder Repairable(bool value = true) => RepairableValue(value);
    }
}
