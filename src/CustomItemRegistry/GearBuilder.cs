namespace ValheimCustomItemRegistry
{
    /// <summary>
    /// Fluent builder for common Valheim gear and shared-data fields.
    /// </summary>
    public sealed class GearBuilder
    {
        private readonly CustomItemDefinition definition;

        internal GearBuilder(CustomItemDefinition definition)
        {
            this.definition = definition;
        }

        public GearBuilder OneHandedWeapon() => Type(ItemDrop.ItemData.ItemType.OneHandedWeapon);
        public GearBuilder TwoHandedWeapon() => Type(ItemDrop.ItemData.ItemType.TwoHandedWeapon);
        public GearBuilder Shield() => Type(ItemDrop.ItemData.ItemType.Shield);
        public GearBuilder Bow() => Type(ItemDrop.ItemData.ItemType.Bow);
        public GearBuilder Tool() => Type(ItemDrop.ItemData.ItemType.Tool);
        public GearBuilder Armor() => Type(ItemDrop.ItemData.ItemType.Chest);
        public GearBuilder Helmet() => Type(ItemDrop.ItemData.ItemType.Helmet);
        public GearBuilder Chest() => Type(ItemDrop.ItemData.ItemType.Chest);
        public GearBuilder Legs() => Type(ItemDrop.ItemData.ItemType.Legs);
        public GearBuilder Utility() => Type(ItemDrop.ItemData.ItemType.Utility);

        public GearBuilder Type(ItemDrop.ItemData.ItemType itemType)
        {
            definition.ItemType = itemType;
            return this;
        }

        public GearBuilder Weight(float value)
        {
            definition.Weight = value;
            return this;
        }

        public GearBuilder StackSize(int value)
        {
            definition.StackSize = value;
            return this;
        }

        public GearBuilder Durability(float value)
        {
            definition.MaxDurability = value;
            return this;
        }

        public GearBuilder DurabilityPerLevel(float value)
        {
            definition.DurabilityPerLevel = value;
            return this;
        }

        public GearBuilder MaxQuality(int value)
        {
            definition.MaxQuality = value;
            return this;
        }

        public GearBuilder ToolTier(int value)
        {
            definition.ToolTier = value;
            return this;
        }

        public GearBuilder ArmorValue(float value)
        {
            definition.Armor = value;
            return this;
        }

        public GearBuilder ArmorPerLevel(float value)
        {
            definition.ArmorPerLevel = value;
            return this;
        }

        public GearBuilder BlockPower(float value)
        {
            definition.BlockPower = value;
            return this;
        }

        public GearBuilder BlockPowerPerLevel(float value)
        {
            definition.BlockPowerPerLevel = value;
            return this;
        }

        public GearBuilder BlockForce(float value)
        {
            definition.DeflectionForce = value;
            return this;
        }

        public GearBuilder BlockForcePerLevel(float value)
        {
            definition.DeflectionForcePerLevel = value;
            return this;
        }

        public GearBuilder MovementModifier(float value)
        {
            definition.MovementModifier = value;
            return this;
        }

        public GearBuilder Teleportable(bool value = true)
        {
            definition.Teleportable = value;
            return this;
        }

        public GearBuilder Repairable(bool value = true)
        {
            definition.CanBeRepaired = value;
            return this;
        }

        public GearBuilder BluntDamage(float value) => Damage(d => { d.m_blunt = value; return d; });
        public GearBuilder SlashDamage(float value) => Damage(d => { d.m_slash = value; return d; });
        public GearBuilder PierceDamage(float value) => Damage(d => { d.m_pierce = value; return d; });
        public GearBuilder FireDamage(float value) => Damage(d => { d.m_fire = value; return d; });
        public GearBuilder FrostDamage(float value) => Damage(d => { d.m_frost = value; return d; });
        public GearBuilder LightningDamage(float value) => Damage(d => { d.m_lightning = value; return d; });
        public GearBuilder PoisonDamage(float value) => Damage(d => { d.m_poison = value; return d; });
        public GearBuilder SpiritDamage(float value) => Damage(d => { d.m_spirit = value; return d; });
        public GearBuilder ChopDamage(float value) => Damage(d => { d.m_chop = value; return d; });
        public GearBuilder PickaxeDamage(float value) => Damage(d => { d.m_pickaxe = value; return d; });

        public GearBuilder BluntDamagePerLevel(float value) => DamagePerLevel(d => { d.m_blunt = value; return d; });
        public GearBuilder SlashDamagePerLevel(float value) => DamagePerLevel(d => { d.m_slash = value; return d; });
        public GearBuilder PierceDamagePerLevel(float value) => DamagePerLevel(d => { d.m_pierce = value; return d; });
        public GearBuilder FireDamagePerLevel(float value) => DamagePerLevel(d => { d.m_fire = value; return d; });
        public GearBuilder FrostDamagePerLevel(float value) => DamagePerLevel(d => { d.m_frost = value; return d; });
        public GearBuilder LightningDamagePerLevel(float value) => DamagePerLevel(d => { d.m_lightning = value; return d; });
        public GearBuilder PoisonDamagePerLevel(float value) => DamagePerLevel(d => { d.m_poison = value; return d; });
        public GearBuilder SpiritDamagePerLevel(float value) => DamagePerLevel(d => { d.m_spirit = value; return d; });
        public GearBuilder ChopDamagePerLevel(float value) => DamagePerLevel(d => { d.m_chop = value; return d; });
        public GearBuilder PickaxeDamagePerLevel(float value) => DamagePerLevel(d => { d.m_pickaxe = value; return d; });

        private GearBuilder Damage(System.Func<HitData.DamageTypes, HitData.DamageTypes> configure)
        {
            definition.Damages = configure(definition.Damages);
            definition.HasDamages = true;
            return this;
        }

        private GearBuilder DamagePerLevel(System.Func<HitData.DamageTypes, HitData.DamageTypes> configure)
        {
            definition.DamagesPerLevel = configure(definition.DamagesPerLevel);
            definition.HasDamagesPerLevel = true;
            return this;
        }
    }
}
