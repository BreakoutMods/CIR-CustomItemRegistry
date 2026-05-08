namespace ValheimCustomItemRegistry
{
    /// <summary>
    /// Shared helpers for the strongly typed item template builders.
    /// </summary>
    public abstract class TemplateBuilderBase<TBuilder>
        where TBuilder : TemplateBuilderBase<TBuilder>
    {
        protected readonly CustomItemDefinition Definition;

        protected TemplateBuilderBase(CustomItemDefinition definition, string templateName)
        {
            Definition = definition;
            Definition.TemplateName = templateName;
        }

        public TBuilder Weight(float value)
        {
            Definition.Weight = value;
            return This();
        }

        public TBuilder StackSize(int value)
        {
            Definition.StackSize = value;
            return This();
        }

        public TBuilder Movement(float value)
        {
            Definition.MovementModifier = value;
            return This();
        }

        public TBuilder Teleportable(bool value = true)
        {
            Definition.Teleportable = value;
            return This();
        }

        public TBuilder ConfigureSharedData(System.Action<ItemDrop.ItemData.SharedData> configure)
        {
            if (configure != null)
            {
                Definition.SharedDataConfigurators.Add(configure);
            }

            return This();
        }

        protected TBuilder DurabilityValues(float value, float perLevel)
        {
            Definition.MaxDurability = value;
            Definition.DurabilityPerLevel = perLevel;
            return This();
        }

        protected TBuilder MaxQualityValue(int value)
        {
            Definition.MaxQuality = value;
            return This();
        }

        protected TBuilder RepairableValue(bool value)
        {
            Definition.CanBeRepaired = value;
            return This();
        }

        protected TBuilder Damage(System.Func<HitData.DamageTypes, HitData.DamageTypes> configure)
        {
            Definition.Damages = configure(Definition.Damages);
            Definition.HasDamages = true;
            return This();
        }

        protected TBuilder DamagePerLevel(System.Func<HitData.DamageTypes, HitData.DamageTypes> configure)
        {
            Definition.DamagesPerLevel = configure(Definition.DamagesPerLevel);
            Definition.HasDamagesPerLevel = true;
            return This();
        }

        protected TBuilder PrimaryAttack(System.Action<Attack> configure)
        {
            Definition.SharedDataConfigurators.Add(shared => ConfigureAttack(shared.m_attack, "primary", shared.m_name, configure));
            return This();
        }

        protected TBuilder SecondaryAttack(System.Action<Attack> configure)
        {
            Definition.SharedDataConfigurators.Add(shared => ConfigureAttack(shared.m_secondaryAttack, "secondary", shared.m_name, configure));
            return This();
        }

        protected TBuilder This()
        {
            return (TBuilder)this;
        }

        private static void ConfigureAttack(Attack attack, string label, string itemName, System.Action<Attack> configure)
        {
            if (attack == null)
            {
                throw new CustomItemRegistrationException($"Item '{itemName}' has no {label} attack to configure");
            }

            configure(attack);
        }
    }
}
