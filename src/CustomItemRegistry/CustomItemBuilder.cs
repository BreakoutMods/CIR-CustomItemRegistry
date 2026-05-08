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
