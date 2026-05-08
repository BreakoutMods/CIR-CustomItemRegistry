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
        /// Load a prefab from an AssetBundle, clone it as itemName, create a recipe, and register it with Valheim.
        /// </summary>
        public static void RegisterItem(string itemName, string assetBundlePath, string prefabName, CraftingRecipe recipe)
        {
            ValidateRegistration(itemName, assetBundlePath, prefabName);

            if (RegisteredItems.ContainsKey(itemName))
            {
                LogWarning($"Item '{itemName}' is already registered");
                return;
            }

            AssetBundle assetBundle = LoadAssetBundle(assetBundlePath);
            GameObject sourcePrefab = assetBundle.LoadAsset<GameObject>(prefabName);
            if (!sourcePrefab)
            {
                throw new InvalidOperationException($"AssetBundle '{assetBundlePath}' does not contain prefab '{prefabName}'");
            }

            GameObject itemPrefab = Object.Instantiate(sourcePrefab);
            itemPrefab.name = itemName;
            itemPrefab.SetActive(false);

            PrepareItemPrefab(itemPrefab);

            ItemConfig itemConfig = CreateItemConfig(itemName, recipe);
            CustomItem customItem = new CustomItem(itemPrefab, true, itemConfig);

            if (!ItemManager.Instance.AddItem(customItem))
            {
                Object.Destroy(itemPrefab);
                throw new InvalidOperationException($"Jotunn rejected custom item '{itemName}'");
            }

            RegisteredItems.Add(itemName, new RegisteredItem(itemName, itemPrefab, customItem));
            FlushLiveRegistrations();
            LogInfo($"Registered custom item '{itemName}' from prefab '{prefabName}'");
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

        private static void ValidateRegistration(string itemName, string assetBundlePath, string prefabName)
        {
            if (string.IsNullOrWhiteSpace(itemName))
            {
                throw new ArgumentException("Item name is required", nameof(itemName));
            }

            if (string.IsNullOrWhiteSpace(assetBundlePath))
            {
                throw new ArgumentException("AssetBundle path is required", nameof(assetBundlePath));
            }

            if (string.IsNullOrWhiteSpace(prefabName))
            {
                throw new ArgumentException("Prefab name is required", nameof(prefabName));
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
                throw new InvalidOperationException($"Prefab '{itemPrefab.name}' must include an ItemDrop component");
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

        private static ItemConfig CreateItemConfig(string itemName, CraftingRecipe recipe)
        {
            List<Ingredient> ingredients = recipe.ingredients ?? new List<Ingredient>();

            return new ItemConfig
            {
                Amount = recipe.amount > 0 ? recipe.amount : 1,
                CraftingStation = recipe.craftingStation,
                Requirements = ingredients
                    .Where(ingredient => !string.IsNullOrWhiteSpace(ingredient.itemName))
                    .Select(ingredient => new RequirementConfig(
                        ingredient.itemName,
                        ingredient.amount > 0 ? ingredient.amount : 1,
                        ingredient.amountPerLevel))
                    .ToArray()
            };
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
