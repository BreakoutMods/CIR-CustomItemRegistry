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
        public GearBuilder TwoHandedWeaponLeft() => Type(ItemDrop.ItemData.ItemType.TwoHandedWeaponLeft);
        public GearBuilder Shield() => Type(ItemDrop.ItemData.ItemType.Shield);
        public GearBuilder Bow() => Type(ItemDrop.ItemData.ItemType.Bow);
        public GearBuilder Ammo() => Type(ItemDrop.ItemData.ItemType.Ammo);
        public GearBuilder AmmoNonEquipable() => Type(ItemDrop.ItemData.ItemType.AmmoNonEquipable);
        public GearBuilder Material() => Type(ItemDrop.ItemData.ItemType.Material);
        public GearBuilder Consumable() => Type(ItemDrop.ItemData.ItemType.Consumable);
        public GearBuilder Torch() => Type(ItemDrop.ItemData.ItemType.Torch);
        public GearBuilder Tool() => Type(ItemDrop.ItemData.ItemType.Tool);
        public GearBuilder Armor() => Type(ItemDrop.ItemData.ItemType.Chest);
        public GearBuilder Helmet() => Type(ItemDrop.ItemData.ItemType.Helmet);
        public GearBuilder Chest() => Type(ItemDrop.ItemData.ItemType.Chest);
        public GearBuilder Legs() => Type(ItemDrop.ItemData.ItemType.Legs);
        public GearBuilder Shoulder() => Type(ItemDrop.ItemData.ItemType.Shoulder);
        public GearBuilder Utility() => Type(ItemDrop.ItemData.ItemType.Utility);
        public GearBuilder Trinket() => Type(ItemDrop.ItemData.ItemType.Trinket);

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

        public GearBuilder Parry(float value)
        {
            definition.SharedDataConfigurators.Add(shared => shared.m_timedBlockBonus = value);
            return this;
        }

        public GearBuilder AttackForce(float value)
        {
            definition.SharedDataConfigurators.Add(shared => shared.m_attackForce = value);
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

        public GearBuilder DamageModifier(HitData.DamageType damageType, HitData.DamageModifier modifier)
        {
            definition.SharedDataConfigurators.Add(shared =>
            {
                if (shared.m_damageModifiers == null)
                {
                    shared.m_damageModifiers = new System.Collections.Generic.List<HitData.DamageModPair>();
                }

                shared.m_damageModifiers.RemoveAll(entry => entry.m_type == damageType);
                shared.m_damageModifiers.Add(new HitData.DamageModPair
                {
                    m_type = damageType,
                    m_modifier = modifier
                });
            });
            return this;
        }

        public GearBuilder PrimaryAttackStamina(float value) => PrimaryAttack(attack => attack.m_attackStamina = value);
        public GearBuilder PrimaryAttackEitr(float value) => PrimaryAttack(attack => attack.m_attackEitr = value);
        public GearBuilder PrimaryAttackHealth(float value) => PrimaryAttack(attack => attack.m_attackHealth = value);
        public GearBuilder PrimaryAttackHealthPercentage(float value) => PrimaryAttack(attack => attack.m_attackHealthPercentage = value);
        public GearBuilder PrimaryAttackHealthReturnHit(float value) => PrimaryAttack(attack => attack.m_attackHealthReturnHit = value);
        public GearBuilder PrimaryAttackDamageMultiplierPerMissingHp(float value) => PrimaryAttack(attack => attack.m_damageMultiplierPerMissingHP = value);
        public GearBuilder PrimaryAttackForceMultiplier(float value) => PrimaryAttack(attack => attack.m_forceMultiplier = value);
        public GearBuilder PrimaryAttackProjectileCount(int value) => PrimaryAttack(attack => attack.m_projectiles = value);
        public GearBuilder ProjectileVelocity(float value) => PrimaryAttack(attack => attack.m_projectileVel = value);
        public GearBuilder ProjectileAccuracy(float value) => PrimaryAttack(attack => attack.m_projectileAccuracy = value);
        public GearBuilder DrawDuration(float value) => PrimaryAttack(attack => attack.m_drawDurationMin = value);
        public GearBuilder DrawStaminaDrain(float value) => PrimaryAttack(attack => attack.m_drawStaminaDrain = value);
        public GearBuilder ReloadTime(float value) => PrimaryAttack(attack => attack.m_reloadTime = value);
        public GearBuilder ReloadStaminaDrain(float value) => PrimaryAttack(attack => attack.m_reloadStaminaDrain = value);

        public GearBuilder SecondaryAttackStamina(float value) => SecondaryAttack(attack => attack.m_attackStamina = value);
        public GearBuilder SecondaryAttackEitr(float value) => SecondaryAttack(attack => attack.m_attackEitr = value);
        public GearBuilder SecondaryAttackHealth(float value) => SecondaryAttack(attack => attack.m_attackHealth = value);
        public GearBuilder SecondaryAttackHealthPercentage(float value) => SecondaryAttack(attack => attack.m_attackHealthPercentage = value);
        public GearBuilder SecondaryAttackForceMultiplier(float value) => SecondaryAttack(attack => attack.m_forceMultiplier = value);

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

        private GearBuilder PrimaryAttack(System.Action<Attack> configure)
        {
            definition.SharedDataConfigurators.Add(shared => ConfigureAttack(shared.m_attack, "primary", shared.m_name, configure));
            return this;
        }

        private GearBuilder SecondaryAttack(System.Action<Attack> configure)
        {
            definition.SharedDataConfigurators.Add(shared => ConfigureAttack(shared.m_secondaryAttack, "secondary", shared.m_name, configure));
            return this;
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
