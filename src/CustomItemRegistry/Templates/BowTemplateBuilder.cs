namespace ValheimCustomItemRegistry
{
    /// <summary>
    /// Template builder for bows and projectile weapons.
    /// </summary>
    public sealed class BowTemplateBuilder : TemplateBuilderBase<BowTemplateBuilder>
    {
        internal BowTemplateBuilder(CustomItemDefinition definition)
            : base(definition, "Bow")
        {
            definition.ItemType = ItemDrop.ItemData.ItemType.Bow;
            definition.StackSize = definition.StackSize ?? 1;
            definition.MaxQuality = definition.MaxQuality ?? 4;
            definition.MaxDurability = definition.MaxDurability ?? 200f;
            definition.DurabilityPerLevel = definition.DurabilityPerLevel ?? 50f;
            definition.CanBeRepaired = definition.CanBeRepaired ?? true;
            definition.TemplateRequiresDamage = true;
        }

        public BowTemplateBuilder Pierce(float value, float perLevel = 0f)
        {
            Damage(d => { d.m_pierce = value; return d; });
            if (perLevel != 0f)
            {
                DamagePerLevel(d => { d.m_pierce = perLevel; return d; });
            }

            return this;
        }

        public BowTemplateBuilder Attack(float stamina, float drawDuration = 1f, float drawStaminaDrain = 6f)
        {
            PrimaryAttack(attack =>
            {
                attack.m_attackStamina = stamina;
                attack.m_drawDurationMin = drawDuration;
                attack.m_drawStaminaDrain = drawStaminaDrain;
            });
            return this;
        }

        public BowTemplateBuilder Projectile(float velocity, float accuracy = 0f)
        {
            PrimaryAttack(attack =>
            {
                attack.m_projectileVel = velocity;
                attack.m_projectileAccuracy = accuracy;
            });
            return this;
        }

        public BowTemplateBuilder Durability(float value, float perLevel = 0f) => DurabilityValues(value, perLevel);
        public BowTemplateBuilder MaxQuality(int value) => MaxQualityValue(value);
        public BowTemplateBuilder Repairable(bool value = true) => RepairableValue(value);
    }
}
