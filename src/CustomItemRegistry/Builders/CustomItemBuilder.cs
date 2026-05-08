using System;
using System.Reflection;
using Jotunn.Utils;
using UnityEngine;

namespace ValheimCustomItemRegistry
{
    /// <summary>
    /// Fluent builder for defining and registering custom items.
    /// </summary>
    public sealed class CustomItemBuilder
    {
        private readonly CustomItemDefinition definition;

        internal CustomItemBuilder(string itemName)
        {
            definition = new CustomItemDefinition(itemName);
        }

        public CustomItemBuilder FromBundle(string assetBundlePath, string prefabName)
        {
            definition.AssetBundlePath = assetBundlePath;
            definition.AssetBundle = null;
            definition.PrefabName = prefabName;
            return this;
        }

        public CustomItemBuilder FromAssetBundle(AssetBundle assetBundle, string prefabName)
        {
            definition.AssetBundle = assetBundle;
            definition.AssetBundlePath = null;
            definition.PrefabName = prefabName;
            return this;
        }

        public CustomItemBuilder FromEmbeddedResource(string resourceName, Assembly assembly, string prefabName)
        {
            definition.AssetBundle = AssetUtils.LoadAssetBundleFromResources(resourceName, assembly);
            definition.AssetBundlePath = resourceName;
            definition.PrefabName = prefabName;
            return this;
        }

        public CustomItemBuilder DisplayName(string displayName)
        {
            definition.DisplayName = displayName;
            return this;
        }

        public CustomItemBuilder Description(string description)
        {
            definition.Description = description;
            return this;
        }

        public CustomItemBuilder Icon(string assetName)
        {
            definition.IconAssetName = assetName;
            return this;
        }

        public CustomItemBuilder Icon(Sprite sprite)
        {
            definition.Icon = sprite;
            return this;
        }

        public CustomItemBuilder Recipe(Action<RecipeBuilder> configure)
        {
            RecipeBuilder builder = new RecipeBuilder();
            configure?.Invoke(builder);
            definition.Recipe = builder.Build();
            definition.HasRecipe = true;
            return this;
        }

        public CustomItemBuilder Gear(Action<GearBuilder> configure)
        {
            GearBuilder builder = new GearBuilder(definition);
            configure?.Invoke(builder);
            return this;
        }

        public CustomItemBuilder AsSword(Action<WeaponTemplateBuilder> configure)
        {
            WeaponTemplateBuilder builder = new WeaponTemplateBuilder(definition, "Sword", ItemDrop.ItemData.ItemType.OneHandedWeapon);
            configure?.Invoke(builder);
            return this;
        }

        public CustomItemBuilder AsAxe(Action<WeaponTemplateBuilder> configure)
        {
            WeaponTemplateBuilder builder = new WeaponTemplateBuilder(definition, "Axe", ItemDrop.ItemData.ItemType.OneHandedWeapon);
            configure?.Invoke(builder);
            return this;
        }

        public CustomItemBuilder AsMace(Action<WeaponTemplateBuilder> configure)
        {
            WeaponTemplateBuilder builder = new WeaponTemplateBuilder(definition, "Mace", ItemDrop.ItemData.ItemType.OneHandedWeapon);
            configure?.Invoke(builder);
            return this;
        }

        public CustomItemBuilder AsSpear(Action<WeaponTemplateBuilder> configure)
        {
            WeaponTemplateBuilder builder = new WeaponTemplateBuilder(definition, "Spear", ItemDrop.ItemData.ItemType.OneHandedWeapon);
            configure?.Invoke(builder);
            return this;
        }

        public CustomItemBuilder AsKnife(Action<WeaponTemplateBuilder> configure)
        {
            WeaponTemplateBuilder builder = new WeaponTemplateBuilder(definition, "Knife", ItemDrop.ItemData.ItemType.OneHandedWeapon);
            configure?.Invoke(builder);
            return this;
        }

        public CustomItemBuilder AsAtgeir(Action<WeaponTemplateBuilder> configure)
        {
            WeaponTemplateBuilder builder = new WeaponTemplateBuilder(definition, "Atgeir", ItemDrop.ItemData.ItemType.TwoHandedWeapon);
            configure?.Invoke(builder);
            return this;
        }

        public CustomItemBuilder AsBow(Action<BowTemplateBuilder> configure)
        {
            BowTemplateBuilder builder = new BowTemplateBuilder(definition);
            configure?.Invoke(builder);
            return this;
        }

        public CustomItemBuilder AsArrow(Action<AmmoTemplateBuilder> configure)
        {
            AmmoTemplateBuilder builder = new AmmoTemplateBuilder(definition);
            configure?.Invoke(builder);
            return this;
        }

        public CustomItemBuilder AsShield(Action<ShieldTemplateBuilder> configure)
        {
            ShieldTemplateBuilder builder = new ShieldTemplateBuilder(definition);
            configure?.Invoke(builder);
            return this;
        }

        public CustomItemBuilder AsArmorChest(Action<ArmorTemplateBuilder> configure)
        {
            ArmorTemplateBuilder builder = new ArmorTemplateBuilder(definition, "ArmorChest", ItemDrop.ItemData.ItemType.Chest);
            configure?.Invoke(builder);
            return this;
        }

        public CustomItemBuilder AsArmorLegs(Action<ArmorTemplateBuilder> configure)
        {
            ArmorTemplateBuilder builder = new ArmorTemplateBuilder(definition, "ArmorLegs", ItemDrop.ItemData.ItemType.Legs);
            configure?.Invoke(builder);
            return this;
        }

        public CustomItemBuilder AsHelmet(Action<ArmorTemplateBuilder> configure)
        {
            ArmorTemplateBuilder builder = new ArmorTemplateBuilder(definition, "Helmet", ItemDrop.ItemData.ItemType.Helmet);
            configure?.Invoke(builder);
            return this;
        }

        public CustomItemBuilder AsCape(Action<ArmorTemplateBuilder> configure)
        {
            ArmorTemplateBuilder builder = new ArmorTemplateBuilder(definition, "Cape", ItemDrop.ItemData.ItemType.Shoulder);
            configure?.Invoke(builder);
            return this;
        }

        public CustomItemBuilder AsTool(Action<ToolTemplateBuilder> configure)
        {
            ToolTemplateBuilder builder = new ToolTemplateBuilder(definition);
            configure?.Invoke(builder);
            return this;
        }

        public CustomItemBuilder AsFood(Action<FoodTemplateBuilder> configure)
        {
            FoodTemplateBuilder builder = new FoodTemplateBuilder(definition);
            configure?.Invoke(builder);
            return this;
        }

        public CustomItemBuilder AsMaterial(Action<MaterialTemplateBuilder> configure = null)
        {
            MaterialTemplateBuilder builder = new MaterialTemplateBuilder(definition);
            configure?.Invoke(builder);
            return this;
        }

        public CustomItemBuilder ConfigureSharedData(Action<ItemDrop.ItemData.SharedData> configure)
        {
            if (configure != null)
            {
                definition.SharedDataConfigurators.Add(configure);
            }

            return this;
        }

        public CustomItemDefinition Build()
        {
            return definition;
        }

        public ItemRegistrationResult Register()
        {
            return CustomItemRegistry.RegisterItem(definition);
        }
    }
}
