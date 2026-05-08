namespace ValheimCustomItemRegistry
{
    /// <summary>
    /// Template builder for arrows, bolts, and ammo.
    /// </summary>
    public sealed class AmmoTemplateBuilder : TemplateBuilderBase<AmmoTemplateBuilder>
    {
        internal AmmoTemplateBuilder(CustomItemDefinition definition)
            : base(definition, "Arrow")
        {
            definition.ItemType = ItemDrop.ItemData.ItemType.Ammo;
            definition.StackSize = definition.StackSize ?? 100;
            definition.MaxQuality = definition.MaxQuality ?? 1;
            definition.MaxDurability = definition.MaxDurability ?? 0f;
            definition.CanBeRepaired = definition.CanBeRepaired ?? false;
            definition.TemplateRequiresDamage = true;
        }

        public AmmoTemplateBuilder Pierce(float value, float perLevel = 0f) => DamageWithLevel(perLevel, d => { d.m_pierce = value; return d; }, d => { d.m_pierce = perLevel; return d; });
        public AmmoTemplateBuilder Blunt(float value, float perLevel = 0f) => DamageWithLevel(perLevel, d => { d.m_blunt = value; return d; }, d => { d.m_blunt = perLevel; return d; });
        public AmmoTemplateBuilder Slash(float value, float perLevel = 0f) => DamageWithLevel(perLevel, d => { d.m_slash = value; return d; }, d => { d.m_slash = perLevel; return d; });
        public AmmoTemplateBuilder Fire(float value, float perLevel = 0f) => DamageWithLevel(perLevel, d => { d.m_fire = value; return d; }, d => { d.m_fire = perLevel; return d; });
        public AmmoTemplateBuilder Frost(float value, float perLevel = 0f) => DamageWithLevel(perLevel, d => { d.m_frost = value; return d; }, d => { d.m_frost = perLevel; return d; });
        public AmmoTemplateBuilder Lightning(float value, float perLevel = 0f) => DamageWithLevel(perLevel, d => { d.m_lightning = value; return d; }, d => { d.m_lightning = perLevel; return d; });
        public AmmoTemplateBuilder Poison(float value, float perLevel = 0f) => DamageWithLevel(perLevel, d => { d.m_poison = value; return d; }, d => { d.m_poison = perLevel; return d; });
        public AmmoTemplateBuilder Spirit(float value, float perLevel = 0f) => DamageWithLevel(perLevel, d => { d.m_spirit = value; return d; }, d => { d.m_spirit = perLevel; return d; });

        public AmmoTemplateBuilder NonEquipable(bool value = true)
        {
            if (value)
            {
                Definition.ItemType = ItemDrop.ItemData.ItemType.AmmoNonEquipable;
            }

            return this;
        }

        private AmmoTemplateBuilder DamageWithLevel(
            float perLevel,
            System.Func<HitData.DamageTypes, HitData.DamageTypes> setDamage,
            System.Func<HitData.DamageTypes, HitData.DamageTypes> setPerLevel)
        {
            Damage(setDamage);
            if (perLevel != 0f)
            {
                DamagePerLevel(setPerLevel);
            }

            return this;
        }
    }
}
