namespace ValheimCustomItemRegistry
{
    /// <summary>
    /// Template builder for melee weapons.
    /// </summary>
    public sealed class WeaponTemplateBuilder : TemplateBuilderBase<WeaponTemplateBuilder>
    {
        internal WeaponTemplateBuilder(CustomItemDefinition definition, string templateName, ItemDrop.ItemData.ItemType itemType)
            : base(definition, templateName)
        {
            definition.ItemType = itemType;
            definition.StackSize = definition.StackSize ?? 1;
            definition.MaxQuality = definition.MaxQuality ?? 4;
            definition.MaxDurability = definition.MaxDurability ?? 200f;
            definition.DurabilityPerLevel = definition.DurabilityPerLevel ?? 50f;
            definition.CanBeRepaired = definition.CanBeRepaired ?? true;
            definition.TemplateRequiresDamage = true;
        }

        public WeaponTemplateBuilder Slash(float value, float perLevel = 0f) => DamageWithLevel(perLevel, d => { d.m_slash = value; return d; }, d => { d.m_slash = perLevel; return d; });
        public WeaponTemplateBuilder Blunt(float value, float perLevel = 0f) => DamageWithLevel(perLevel, d => { d.m_blunt = value; return d; }, d => { d.m_blunt = perLevel; return d; });
        public WeaponTemplateBuilder Pierce(float value, float perLevel = 0f) => DamageWithLevel(perLevel, d => { d.m_pierce = value; return d; }, d => { d.m_pierce = perLevel; return d; });
        public WeaponTemplateBuilder Chop(float value, float perLevel = 0f) => DamageWithLevel(perLevel, d => { d.m_chop = value; return d; }, d => { d.m_chop = perLevel; return d; });
        public WeaponTemplateBuilder Pickaxe(float value, float perLevel = 0f) => DamageWithLevel(perLevel, d => { d.m_pickaxe = value; return d; }, d => { d.m_pickaxe = perLevel; return d; });
        public WeaponTemplateBuilder Fire(float value, float perLevel = 0f) => DamageWithLevel(perLevel, d => { d.m_fire = value; return d; }, d => { d.m_fire = perLevel; return d; });
        public WeaponTemplateBuilder Frost(float value, float perLevel = 0f) => DamageWithLevel(perLevel, d => { d.m_frost = value; return d; }, d => { d.m_frost = perLevel; return d; });
        public WeaponTemplateBuilder Lightning(float value, float perLevel = 0f) => DamageWithLevel(perLevel, d => { d.m_lightning = value; return d; }, d => { d.m_lightning = perLevel; return d; });
        public WeaponTemplateBuilder Poison(float value, float perLevel = 0f) => DamageWithLevel(perLevel, d => { d.m_poison = value; return d; }, d => { d.m_poison = perLevel; return d; });
        public WeaponTemplateBuilder Spirit(float value, float perLevel = 0f) => DamageWithLevel(perLevel, d => { d.m_spirit = value; return d; }, d => { d.m_spirit = perLevel; return d; });

        public WeaponTemplateBuilder Block(float power, float force = 0f, float parry = 1.5f)
        {
            Definition.BlockPower = power;
            Definition.DeflectionForce = force;
            Definition.SharedDataConfigurators.Add(shared => shared.m_timedBlockBonus = parry);
            return this;
        }

        public WeaponTemplateBuilder Attack(float stamina, float force = 0f, float eitr = 0f)
        {
            Definition.SharedDataConfigurators.Add(shared => shared.m_attackForce = force);
            PrimaryAttack(attack =>
            {
                attack.m_attackStamina = stamina;
                attack.m_attackEitr = eitr;
            });
            return this;
        }

        public WeaponTemplateBuilder SecondaryAttack(float stamina, float forceMultiplier = 1f, float eitr = 0f)
        {
            return base.SecondaryAttack(attack =>
            {
                attack.m_attackStamina = stamina;
                attack.m_attackEitr = eitr;
                attack.m_forceMultiplier = forceMultiplier;
            });
        }

        public WeaponTemplateBuilder Durability(float value, float perLevel = 0f) => DurabilityValues(value, perLevel);
        public WeaponTemplateBuilder MaxQuality(int value) => MaxQualityValue(value);
        public WeaponTemplateBuilder Repairable(bool value = true) => RepairableValue(value);
        public WeaponTemplateBuilder ToolTier(int value)
        {
            Definition.ToolTier = value;
            return this;
        }

        private WeaponTemplateBuilder DamageWithLevel(
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
