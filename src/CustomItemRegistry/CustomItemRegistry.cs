using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using BepInEx.Logging;
using Jotunn;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ValheimCustomItemRegistry
{
    /// <summary>
    /// Public modding API for loading, cloning, and registering custom 3D item prefabs.
    /// </summary>
    public static class CustomItemRegistry
    {
        private static readonly Dictionary<string, RegisteredItem> RegisteredItems = new Dictionary<string, RegisteredItem>();
        private static readonly Dictionary<string, AssetBundle> LoadedAssetBundles = new Dictionary<string, AssetBundle>();

        private static ManualLogSource logger;

        /// <summary>
        /// Start a fluent CIR 0.2 custom item definition.
        /// </summary>
        public static CustomItemBuilder Item(string itemName)
        {
            return new CustomItemBuilder(itemName);
        }

        /// <summary>
        /// Load a prefab from an AssetBundle, clone it as itemName, create a recipe, and register it with Valheim.
        /// </summary>
        public static void RegisterItem(string itemName, string assetBundlePath, string prefabName, CraftingRecipe recipe)
        {
            if (RegisteredItems.ContainsKey(itemName))
            {
                LogWarning($"Item '{itemName}' is already registered");
                return;
            }

            RegisterItem(new CustomItemDefinition(itemName)
            {
                AssetBundlePath = assetBundlePath,
                PrefabName = prefabName,
                Recipe = recipe,
                HasRecipe = true
            });
        }

        /// <summary>
        /// Register a complete CIR 0.2 custom item definition.
        /// </summary>
        public static ItemRegistrationResult RegisterItem(CustomItemDefinition definition)
        {
            GameObject itemPrefab = null;

            try
            {
                ValidateDefinition(definition);

                AssetBundle assetBundle = LoadAssetBundle(definition.AssetBundlePath);
                GameObject sourcePrefab = assetBundle.LoadAsset<GameObject>(definition.PrefabName);
                if (!sourcePrefab)
                {
                    throw new CustomItemRegistrationException(definition, "AssetBundle does not contain the requested prefab");
                }

                itemPrefab = Object.Instantiate(sourcePrefab);
                itemPrefab.name = definition.ItemName;
                itemPrefab.SetActive(false);

                PrepareItemPrefab(itemPrefab);
                LoadIcon(definition, assetBundle);

                ItemConfig itemConfig = CreateItemConfig(definition);
                CustomItem customItem = new CustomItem(itemPrefab, true, itemConfig);

                ApplyGearMetadata(definition, itemPrefab);
                ApplySharedDataConfigurators(definition, itemPrefab);
                ValidatePreparedItem(definition, itemPrefab, customItem);
                WarnForMissingIngredients(definition);

                if (!ItemManager.Instance.AddItem(customItem))
                {
                    throw new CustomItemRegistrationException(definition, "Jotunn rejected the custom item");
                }

                RegisteredItems.Add(definition.ItemName, new RegisteredItem(definition.ItemName, itemPrefab, customItem));
                FlushLiveRegistrations();

                ItemRegistrationResult result = ItemRegistrationResult.Registered(definition, itemPrefab, customItem);
                LogInfo($"Registered custom item '{definition.ItemName}' from bundle '{definition.AssetBundlePath}' prefab '{definition.PrefabName}'");
                return result;
            }
            catch (Exception exception)
            {
                if (itemPrefab)
                {
                    Object.Destroy(itemPrefab);
                }

                CustomItemRegistrationException existingRegistrationException = exception as CustomItemRegistrationException;
                CustomItemRegistrationException registrationException = existingRegistrationException != null && !string.IsNullOrEmpty(existingRegistrationException.ItemName)
                    ? existingRegistrationException
                    : new CustomItemRegistrationException(definition, exception.Message, exception);

                LogWarning(registrationException.Message);
                throw registrationException;
            }
        }

        /// <summary>
        /// Try to register a custom item without throwing on validation or registration failure.
        /// </summary>
        public static bool TryRegisterItem(CustomItemDefinition definition, out ItemRegistrationResult result)
        {
            try
            {
                result = RegisterItem(definition);
                return true;
            }
            catch (Exception exception)
            {
                result = ItemRegistrationResult.Failed(definition, exception);
                return false;
            }
        }

        /// <summary>
        /// Register several item definitions in order. Throws on the first failed item.
        /// </summary>
        public static IReadOnlyList<ItemRegistrationResult> RegisterItems(IEnumerable<CustomItemDefinition> definitions)
        {
            if (definitions == null)
            {
                throw new ArgumentNullException(nameof(definitions));
            }

            List<ItemRegistrationResult> results = new List<ItemRegistrationResult>();
            foreach (CustomItemDefinition definition in definitions)
            {
                results.Add(RegisterItem(definition));
            }

            return results;
        }

        internal static void SetLogger(ManualLogSource manualLogSource)
        {
            logger = manualLogSource;
        }

        internal static void FlushLiveRegistrations(ObjectDB objectDB = null, ZNetScene zNetScene = null)
        {
            if (RegisteredItems.Count == 0)
            {
                return;
            }

            ObjectDB liveObjectDB = objectDB ? objectDB : ObjectDB.instance;
            ZNetScene liveZNetScene = zNetScene ? zNetScene : ZNetScene.instance;

            foreach (RegisteredItem item in RegisteredItems.Values)
            {
                if (liveZNetScene)
                {
                    PrefabManager.Instance.RegisterToZNetScene(item.Prefab);
                }

                if (liveObjectDB)
                {
                    RegisterPrefabInObjectDB(item);
                    RegisterRecipeInObjectDB(liveObjectDB, item);
                }
            }
        }

        internal static void UnloadAssetBundles()
        {
            foreach (AssetBundle bundle in LoadedAssetBundles.Values)
            {
                if (bundle)
                {
                    bundle.Unload(false);
                }
            }

            LoadedAssetBundles.Clear();
        }

        private static void ValidateDefinition(CustomItemDefinition definition)
        {
            if (definition == null)
            {
                throw new CustomItemRegistrationException("Custom item definition is required");
            }

            if (string.IsNullOrWhiteSpace(definition.ItemName))
            {
                throw new CustomItemRegistrationException(definition, "Item name is required");
            }

            if (string.IsNullOrWhiteSpace(definition.AssetBundlePath))
            {
                throw new CustomItemRegistrationException(definition, "AssetBundle path is required");
            }

            if (string.IsNullOrWhiteSpace(definition.PrefabName))
            {
                throw new CustomItemRegistrationException(definition, "Prefab name is required");
            }

            if (RegisteredItems.ContainsKey(definition.ItemName))
            {
                throw new CustomItemRegistrationException(definition, "Item name is already registered");
            }

            ValidateRecipe(definition);
            ValidateGear(definition);
        }

        private static void ValidateRecipe(CustomItemDefinition definition)
        {
            if (!definition.HasRecipe)
            {
                return;
            }

            CraftingRecipe recipe = definition.Recipe;
            if (recipe.amount <= 0)
            {
                throw new CustomItemRegistrationException(definition, "Recipe amount must be greater than zero");
            }

            if (recipe.minStationLevel < 0)
            {
                throw new CustomItemRegistrationException(definition, "Recipe minimum station level cannot be negative");
            }

            if (recipe.qualityResultAmountMultiplier < 0)
            {
                throw new CustomItemRegistrationException(definition, "Recipe quality result amount multiplier cannot be negative");
            }

            if (recipe.ingredients == null)
            {
                return;
            }

            foreach (Ingredient ingredient in recipe.ingredients)
            {
                if (string.IsNullOrWhiteSpace(ingredient.itemName))
                {
                    throw new CustomItemRegistrationException(definition, "Recipe contains an ingredient with an empty item name");
                }

                if (ingredient.amount <= 0)
                {
                    throw new CustomItemRegistrationException(definition, $"Recipe ingredient '{ingredient.itemName}' amount must be greater than zero");
                }

                if (ingredient.amountPerLevel < 0)
                {
                    throw new CustomItemRegistrationException(definition, $"Recipe ingredient '{ingredient.itemName}' amount per level cannot be negative");
                }
            }
        }

        private static void ValidateGear(CustomItemDefinition definition)
        {
            if (definition.Weight.HasValue && definition.Weight.Value < 0f)
            {
                throw new CustomItemRegistrationException(definition, "Weight cannot be negative");
            }

            if (definition.StackSize.HasValue && definition.StackSize.Value < 1)
            {
                throw new CustomItemRegistrationException(definition, "Stack size must be greater than zero");
            }

            if (definition.MaxDurability.HasValue && definition.MaxDurability.Value < 0f)
            {
                throw new CustomItemRegistrationException(definition, "Durability cannot be negative");
            }

            if (definition.MaxQuality.HasValue && definition.MaxQuality.Value < 1)
            {
                throw new CustomItemRegistrationException(definition, "Max quality must be greater than zero");
            }
        }

        private static AssetBundle LoadAssetBundle(string assetBundlePath)
        {
            string resolvedPath = ResolveAssetBundlePath(assetBundlePath);
            if (!File.Exists(resolvedPath))
            {
                throw new FileNotFoundException($"AssetBundle not found at '{resolvedPath}'", resolvedPath);
            }

            if (LoadedAssetBundles.TryGetValue(resolvedPath, out AssetBundle cachedBundle) && cachedBundle)
            {
                return cachedBundle;
            }

            AssetBundle assetBundle = AssetBundle.LoadFromFile(resolvedPath);
            if (!assetBundle)
            {
                throw new InvalidOperationException($"Failed to load AssetBundle '{resolvedPath}'");
            }

            LoadedAssetBundles[resolvedPath] = assetBundle;
            LogInfo($"Loaded AssetBundle '{resolvedPath}'");
            return assetBundle;
        }

        private static string ResolveAssetBundlePath(string assetBundlePath)
        {
            if (Path.IsPathRooted(assetBundlePath))
            {
                return Path.GetFullPath(assetBundlePath);
            }

            string pluginRelativePath = Path.Combine(Paths.PluginPath, assetBundlePath);
            if (File.Exists(pluginRelativePath))
            {
                return Path.GetFullPath(pluginRelativePath);
            }

            return Path.GetFullPath(assetBundlePath);
        }

        private static void PrepareItemPrefab(GameObject itemPrefab)
        {
            ItemDrop itemDrop = itemPrefab.GetComponent<ItemDrop>();
            if (!itemDrop)
            {
                throw new CustomItemRegistrationException($"Prefab '{itemPrefab.name}' must include an ItemDrop component");
            }

            if (itemDrop.m_itemData.m_shared == null)
            {
                itemDrop.m_itemData.m_shared = new ItemDrop.ItemData.SharedData();
            }

            if (string.IsNullOrEmpty(itemDrop.m_itemData.m_shared.m_name))
            {
                itemDrop.m_itemData.m_shared.m_name = "$" + itemPrefab.name.ToLowerInvariant();
            }

            itemDrop.m_itemData.m_dropPrefab = itemPrefab;

            ZNetView zNetView = itemPrefab.GetComponent<ZNetView>();
            if (!zNetView)
            {
                zNetView = itemPrefab.AddComponent<ZNetView>();
            }

            zNetView.m_persistent = true;
        }

        private static void LoadIcon(CustomItemDefinition definition, AssetBundle assetBundle)
        {
            if (definition.Icon || string.IsNullOrWhiteSpace(definition.IconAssetName))
            {
                return;
            }

            Sprite icon = assetBundle.LoadAsset<Sprite>(definition.IconAssetName);
            if (!icon)
            {
                throw new CustomItemRegistrationException(definition, $"AssetBundle does not contain icon sprite '{definition.IconAssetName}'");
            }

            definition.Icon = icon;
        }

        private static ItemConfig CreateItemConfig(CustomItemDefinition definition)
        {
            CraftingRecipe recipe = definition.Recipe;
            List<Ingredient> ingredients = definition.HasRecipe && recipe.ingredients != null
                ? recipe.ingredients
                : new List<Ingredient>();

            ItemConfig config = new ItemConfig
            {
                Name = definition.DisplayName,
                Description = definition.Description,
                Icon = definition.Icon,
                Amount = definition.HasRecipe ? recipe.amount : 1,
                Enabled = !definition.HasRecipe || recipe.enabled.GetValueOrDefault(true),
                CraftingStation = definition.HasRecipe ? recipe.craftingStation : null,
                RepairStation = definition.HasRecipe ? recipe.repairStation : null,
                MinStationLevel = definition.HasRecipe && recipe.minStationLevel > 0 ? recipe.minStationLevel : 1,
                RequireOnlyOneIngredient = definition.HasRecipe && recipe.requireOnlyOneIngredient,
                QualityResultAmountMultiplier = definition.HasRecipe && recipe.qualityResultAmountMultiplier > 0 ? recipe.qualityResultAmountMultiplier : 1,
                Weight = definition.Weight.GetValueOrDefault(-1f),
                StackSize = definition.StackSize.GetValueOrDefault(-1),
                Requirements = ingredients
                    .Select(ingredient => new RequirementConfig(
                        ingredient.itemName,
                        ingredient.amount,
                        ingredient.amountPerLevel))
                    .ToArray()
            };

            return config;
        }

        private static void ApplyGearMetadata(CustomItemDefinition definition, GameObject itemPrefab)
        {
            ItemDrop itemDrop = itemPrefab.GetComponent<ItemDrop>();
            ItemDrop.ItemData.SharedData shared = itemDrop.m_itemData.m_shared;

            if (definition.ItemType.HasValue) shared.m_itemType = definition.ItemType.Value;
            if (definition.Weight.HasValue) shared.m_weight = definition.Weight.Value;
            if (definition.StackSize.HasValue) shared.m_maxStackSize = definition.StackSize.Value;
            if (definition.MaxDurability.HasValue)
            {
                shared.m_maxDurability = definition.MaxDurability.Value;
                shared.m_useDurability = definition.MaxDurability.Value > 0f;
            }

            if (definition.DurabilityPerLevel.HasValue) shared.m_durabilityPerLevel = definition.DurabilityPerLevel.Value;
            if (definition.MaxQuality.HasValue) shared.m_maxQuality = definition.MaxQuality.Value;
            if (definition.ToolTier.HasValue) shared.m_toolTier = definition.ToolTier.Value;
            if (definition.Armor.HasValue) shared.m_armor = definition.Armor.Value;
            if (definition.ArmorPerLevel.HasValue) shared.m_armorPerLevel = definition.ArmorPerLevel.Value;
            if (definition.BlockPower.HasValue) shared.m_blockPower = definition.BlockPower.Value;
            if (definition.BlockPowerPerLevel.HasValue) shared.m_blockPowerPerLevel = definition.BlockPowerPerLevel.Value;
            if (definition.DeflectionForce.HasValue) shared.m_deflectionForce = definition.DeflectionForce.Value;
            if (definition.DeflectionForcePerLevel.HasValue) shared.m_deflectionForcePerLevel = definition.DeflectionForcePerLevel.Value;
            if (definition.MovementModifier.HasValue) shared.m_movementModifier = definition.MovementModifier.Value;
            if (definition.Teleportable.HasValue) shared.m_teleportable = definition.Teleportable.Value;
            if (definition.CanBeRepaired.HasValue) shared.m_canBeReparied = definition.CanBeRepaired.Value;
            if (definition.HasDamages) shared.m_damages = definition.Damages;
            if (definition.HasDamagesPerLevel) shared.m_damagesPerLevel = definition.DamagesPerLevel;
        }

        private static void ApplySharedDataConfigurators(CustomItemDefinition definition, GameObject itemPrefab)
        {
            if (definition.SharedDataConfigurators.Count == 0)
            {
                return;
            }

            ItemDrop itemDrop = itemPrefab.GetComponent<ItemDrop>();
            ItemDrop.ItemData.SharedData shared = itemDrop.m_itemData.m_shared;
            foreach (Action<ItemDrop.ItemData.SharedData> configure in definition.SharedDataConfigurators)
            {
                configure(shared);
            }
        }

        private static void ValidatePreparedItem(CustomItemDefinition definition, GameObject itemPrefab, CustomItem customItem)
        {
            ItemDrop itemDrop = itemPrefab.GetComponent<ItemDrop>();
            if (!itemDrop)
            {
                throw new CustomItemRegistrationException(definition, "Prepared prefab is missing ItemDrop");
            }

            if (CreatesRecipe(definition))
            {
                Sprite[] icons = itemDrop.m_itemData?.m_shared?.m_icons;
                if (icons == null || icons.Length == 0 || !icons[0])
                {
                    throw new CustomItemRegistrationException(definition, "Craftable items must have an icon; set it in the prefab or call .Icon(...)");
                }
            }

            if (customItem == null || customItem.ItemPrefab == null)
            {
                throw new CustomItemRegistrationException(definition, "Jotunn custom item wrapper was not created");
            }
        }

        private static bool CreatesRecipe(CustomItemDefinition definition)
        {
            return definition.HasRecipe
                && definition.Recipe.ingredients != null
                && definition.Recipe.ingredients.Count > 0;
        }

        private static void WarnForMissingIngredients(CustomItemDefinition definition)
        {
            if (!CreatesRecipe(definition))
            {
                return;
            }

            if (!ObjectDB.instance && !ZNetScene.instance)
            {
                return;
            }

            foreach (Ingredient ingredient in definition.Recipe.ingredients)
            {
                if (!PrefabManager.Instance.GetPrefab(ingredient.itemName))
                {
                    LogWarning($"Item '{definition.ItemName}' recipe ingredient '{ingredient.itemName}' was not found in loaded prefab databases. Jotunn may still resolve it later if another mod registers it.");
                }
            }
        }

        private static void RegisterPrefabInObjectDB(RegisteredItem item)
        {
            if (!ObjectDB.instance || !item.Prefab)
            {
                return;
            }

            ItemManager.Instance.RegisterItemInObjectDB(item.Prefab);
        }

        private static void RegisterRecipeInObjectDB(ObjectDB objectDB, RegisteredItem item)
        {
            CustomRecipe customRecipe = item.CustomItem.Recipe;
            Recipe recipe = customRecipe?.Recipe;
            if (!objectDB || !recipe)
            {
                return;
            }

            if (objectDB.m_recipes.Any(existing => existing && existing.name == recipe.name))
            {
                return;
            }

            if (customRecipe.FixReference || customRecipe.FixRequirementReferences)
            {
                recipe.FixReferences();
                customRecipe.FixReference = false;
                customRecipe.FixRequirementReferences = false;
            }

            objectDB.m_recipes.Add(recipe);
            LogInfo($"Added recipe '{recipe.name}' to ObjectDB");
        }

        private static void LogInfo(string message)
        {
            logger?.LogInfo(message);
        }

        private static void LogWarning(string message)
        {
            logger?.LogWarning(message);
        }

        private sealed class RegisteredItem
        {
            public readonly string ItemName;
            public readonly GameObject Prefab;
            public readonly CustomItem CustomItem;

            public RegisteredItem(string itemName, GameObject prefab, CustomItem customItem)
            {
                ItemName = itemName;
                Prefab = prefab;
                CustomItem = customItem;
            }
        }
    }
}
