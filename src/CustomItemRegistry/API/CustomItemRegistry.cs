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

                AssetBundle assetBundle = GetAssetBundle(definition);
                GameObject sourcePrefab = assetBundle.LoadAsset<GameObject>(definition.PrefabName);
                if (!sourcePrefab)
                {
                    throw new CustomItemRegistrationException(definition, CreateMissingAssetMessage(definition, assetBundle, definition.PrefabName, "prefab"));
                }

                itemPrefab = Object.Instantiate(sourcePrefab);
                itemPrefab.name = definition.ItemName;
                itemPrefab.SetActive(false);

                PrefabPreparationReport preparationReport = PrepareItemPrefab(definition, itemPrefab);
                string iconSource = LoadIcon(definition, assetBundle);

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
                LogRegistrationSuccess(definition, itemPrefab, iconSource, preparationReport);
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

        internal static bool PrepareItemPrefabForTest(CustomItemDefinition definition, GameObject itemPrefab, out IReadOnlyList<string> addedComponents, out IReadOnlyList<string> warnings, out string error)
        {
            addedComponents = Array.Empty<string>();
            warnings = Array.Empty<string>();
            error = null;

            try
            {
                PrefabPreparationReport report = PrepareItemPrefab(definition, itemPrefab);
                addedComponents = report.AddedComponents;
                warnings = report.Warnings;
                return true;
            }
            catch (Exception exception)
            {
                error = exception.Message;
                return false;
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

            if (!definition.AssetBundle && string.IsNullOrWhiteSpace(definition.AssetBundlePath))
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
            if (!string.IsNullOrEmpty(recipe.CraftingStationValidationError))
            {
                throw new CustomItemRegistrationException(definition, recipe.CraftingStationValidationError);
            }

            if (!string.IsNullOrEmpty(recipe.RepairStationValidationError))
            {
                throw new CustomItemRegistrationException(definition, recipe.RepairStationValidationError);
            }

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
                if (!string.IsNullOrEmpty(ingredient.HelperValidationError))
                {
                    throw new CustomItemRegistrationException(definition, ingredient.HelperValidationError);
                }

                if (string.IsNullOrWhiteSpace(ingredient.itemName))
                {
                    throw new CustomItemRegistrationException(definition, "Recipe contains an ingredient with an empty item name");
                }

                if (ingredient.amount < 0)
                {
                    throw new CustomItemRegistrationException(definition, $"Recipe ingredient '{ingredient.itemName}' amount cannot be negative");
                }

                if (ingredient.amountPerLevel < 0)
                {
                    throw new CustomItemRegistrationException(definition, $"Recipe ingredient '{ingredient.itemName}' amount per level cannot be negative");
                }

                if (ingredient.amount == 0 && ingredient.amountPerLevel == 0)
                {
                    throw new CustomItemRegistrationException(definition, $"Recipe ingredient '{ingredient.itemName}' must have a craft amount or upgrade amount per level");
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

            ValidateTemplate(definition);
        }

        private static void ValidateTemplate(CustomItemDefinition definition)
        {
            if (string.IsNullOrEmpty(definition.TemplateName))
            {
                return;
            }

            string templatePrefix = $"Template '{definition.TemplateName}'";
            if (definition.TemplateRequiresDamage && !HasAnyDamage(definition.Damages) && !HasAnyDamage(definition.DamagesPerLevel))
            {
                throw new CustomItemRegistrationException(definition, $"{templatePrefix} requires at least one damage value");
            }

            if (definition.TemplateRequiresBlockPower && (!definition.BlockPower.HasValue || definition.BlockPower.Value <= 0f))
            {
                throw new CustomItemRegistrationException(definition, $"{templatePrefix} requires block power");
            }

            if (definition.TemplateRequiresArmor && (!definition.Armor.HasValue || definition.Armor.Value <= 0f))
            {
                throw new CustomItemRegistrationException(definition, $"{templatePrefix} requires armor value");
            }

            if (definition.TemplateRequiresFoodStats && !definition.TemplateHasFoodStats)
            {
                throw new CustomItemRegistrationException(definition, $"{templatePrefix} requires health, stamina, or eitr food stats");
            }
        }

        private static bool HasAnyDamage(HitData.DamageTypes damages)
        {
            return damages.m_damage > 0f
                || damages.m_blunt > 0f
                || damages.m_slash > 0f
                || damages.m_pierce > 0f
                || damages.m_chop > 0f
                || damages.m_pickaxe > 0f
                || damages.m_fire > 0f
                || damages.m_frost > 0f
                || damages.m_lightning > 0f
                || damages.m_poison > 0f
                || damages.m_spirit > 0f;
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

        private static AssetBundle GetAssetBundle(CustomItemDefinition definition)
        {
            if (definition.AssetBundle)
            {
                return definition.AssetBundle;
            }

            return LoadAssetBundle(definition.AssetBundlePath);
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

        private static PrefabPreparationReport PrepareItemPrefab(CustomItemDefinition definition, GameObject itemPrefab)
        {
            PrefabPreparationOptions options = definition.PrefabPreparation ?? new PrefabPreparationOptions();
            PrefabPreparationReport report = new PrefabPreparationReport();
            List<string> missing = new List<string>();

            ItemDrop itemDrop = itemPrefab.GetComponent<ItemDrop>();
            if (!itemDrop)
            {
                if (options.RequireExistingItemDrop || !options.AutoAddItemDrop)
                {
                    missing.Add(nameof(ItemDrop));
                }
                else
                {
                    itemDrop = itemPrefab.AddComponent<ItemDrop>();
                    report.AddedComponents.Add(nameof(ItemDrop));
                    report.CreatedItemDrop = true;
                }
            }

            Rigidbody rigidbody = itemPrefab.GetComponent<Rigidbody>();
            ZNetView zNetView = itemPrefab.GetComponent<ZNetView>();
            ZSyncTransform zSyncTransform = itemPrefab.GetComponent<ZSyncTransform>();
            bool hasCollider = itemPrefab.GetComponentsInChildren<Collider>(true).Length > 0;

            if (!options.AutoAddPhysics)
            {
                if (!rigidbody) missing.Add(nameof(Rigidbody));
                if (!zNetView) missing.Add(nameof(ZNetView));
                if (!zSyncTransform) missing.Add(nameof(ZSyncTransform));
            }

            if (!hasCollider && !options.WarnOnMissingCollider)
            {
                missing.Add(nameof(Collider));
            }

            if (missing.Count > 0)
            {
                throw new CustomItemRegistrationException(
                    definition,
                    $"Prefab '{definition.PrefabName}' is missing required components: {string.Join(", ", missing.ToArray())}. Add them in Unity, or opt into CIR auto-preparation with .PrefabPreparation(prep => prep.AutoAddItemDrop().AutoAddPhysics().WarnOnMissingCollider().AllowTextureIconFallback()).");
            }

            if (itemDrop.m_itemData == null)
            {
                itemDrop.m_itemData = new ItemDrop.ItemData();
            }

            if (itemDrop.m_itemData.m_shared == null)
            {
                itemDrop.m_itemData.m_shared = new ItemDrop.ItemData.SharedData();
            }

            if (string.IsNullOrEmpty(itemDrop.m_itemData.m_shared.m_name))
            {
                itemDrop.m_itemData.m_shared.m_name = !string.IsNullOrWhiteSpace(definition.DisplayName)
                    ? definition.DisplayName
                    : "$" + definition.ItemName.ToLowerInvariant();
            }

            if (string.IsNullOrEmpty(itemDrop.m_itemData.m_shared.m_description) && !string.IsNullOrWhiteSpace(definition.Description))
            {
                itemDrop.m_itemData.m_shared.m_description = definition.Description;
            }

            if (report.CreatedItemDrop)
            {
                if (!definition.ItemType.HasValue)
                {
                    itemDrop.m_itemData.m_shared.m_itemType = ItemDrop.ItemData.ItemType.Material;
                }

                if (itemDrop.m_itemData.m_shared.m_maxStackSize < 1)
                {
                    itemDrop.m_itemData.m_shared.m_maxStackSize = 1;
                }

                if (itemDrop.m_itemData.m_shared.m_weight <= 0f)
                {
                    itemDrop.m_itemData.m_shared.m_weight = 1f;
                }

                itemDrop.m_itemData.m_shared.m_teleportable = true;
            }

            itemDrop.m_itemData.m_dropPrefab = itemPrefab;

            if (options.AutoAddPhysics)
            {
                if (!rigidbody)
                {
                    rigidbody = itemPrefab.AddComponent<Rigidbody>();
                    report.AddedComponents.Add(nameof(Rigidbody));
                }

                if (!zNetView)
                {
                    zNetView = itemPrefab.AddComponent<ZNetView>();
                    report.AddedComponents.Add(nameof(ZNetView));
                }

                zNetView.m_persistent = true;

                if (!zSyncTransform)
                {
                    zSyncTransform = itemPrefab.AddComponent<ZSyncTransform>();
                    report.AddedComponents.Add(nameof(ZSyncTransform));
                }
            }
            else
            {
                if (zNetView)
                {
                    zNetView.m_persistent = true;
                }
            }

            if (options.WarnOnMissingCollider && !hasCollider)
            {
                string warning = $"Item '{definition.ItemName}' prefab '{definition.PrefabName}' has no Collider. CIR will not add a guessed collider; add one in Unity for reliable pickup/drop physics.";
                report.Warnings.Add(warning);
                LogWarning(warning);
            }

            return report;
        }

        private static string LoadIcon(CustomItemDefinition definition, AssetBundle assetBundle)
        {
            if (definition.Icon)
            {
                return "direct Sprite";
            }

            if (string.IsNullOrWhiteSpace(definition.IconAssetName))
            {
                return "prefab/shared data";
            }

            Sprite icon = assetBundle.LoadAsset<Sprite>(definition.IconAssetName);
            if (icon)
            {
                definition.Icon = icon;
                return $"Sprite '{definition.IconAssetName}'";
            }

            PrefabPreparationOptions options = definition.PrefabPreparation ?? new PrefabPreparationOptions();
            if (options.AllowTextureIconFallback)
            {
                Texture2D texture = assetBundle.LoadAsset<Texture2D>(definition.IconAssetName);
                if (texture)
                {
                    definition.Icon = Sprite.Create(
                        texture,
                        new Rect(0f, 0f, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f));
                    definition.Icon.name = definition.IconAssetName;
                    return $"Texture2D '{definition.IconAssetName}' converted to Sprite";
                }
            }

            throw new CustomItemRegistrationException(definition, CreateMissingAssetMessage(definition, assetBundle, definition.IconAssetName, options.AllowTextureIconFallback ? "icon Sprite or Texture2D" : "icon Sprite"));
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
                        ingredient.amountPerLevel,
                        ingredient.recover))
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

            PrefabPreparationOptions options = definition.PrefabPreparation ?? new PrefabPreparationOptions();
            if (options.ValidateWearableVisuals)
            {
                ValidateWearableVisuals(definition, itemPrefab, itemDrop.m_itemData?.m_shared);
            }
        }

        private static void ValidateWearableVisuals(CustomItemDefinition definition, GameObject itemPrefab, ItemDrop.ItemData.SharedData shared)
        {
            if (shared == null || !IsWearableArmor(shared.m_itemType))
            {
                return;
            }

            SkinnedMeshRenderer[] renderers = itemPrefab.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            if (renderers == null || renderers.Length == 0)
            {
                throw new CustomItemRegistrationException(
                    definition,
                    $"Wearable item prefab '{definition.PrefabName}' is type '{shared.m_itemType}' but has no SkinnedMeshRenderer. Valheim wearable armor needs a skinned visual setup; a normal static mesh can register as a material, but will fail or crash when equipped in SetupVisEquipment.");
            }

            foreach (SkinnedMeshRenderer renderer in renderers)
            {
                string rendererPath = GetTransformPath(renderer.transform, itemPrefab.transform);
                if (renderer.sharedMesh == null)
                {
                    throw new CustomItemRegistrationException(definition, $"Wearable item prefab '{definition.PrefabName}' has SkinnedMeshRenderer '{rendererPath}' without a mesh.");
                }

                if (renderer.rootBone == null)
                {
                    throw new CustomItemRegistrationException(definition, $"Wearable item prefab '{definition.PrefabName}' has SkinnedMeshRenderer '{rendererPath}' without rootBone.");
                }

                if (renderer.bones == null || renderer.bones.Length == 0)
                {
                    throw new CustomItemRegistrationException(definition, $"Wearable item prefab '{definition.PrefabName}' has SkinnedMeshRenderer '{rendererPath}' without bones.");
                }

                if (renderer.bones.Any(bone => bone == null))
                {
                    throw new CustomItemRegistrationException(definition, $"Wearable item prefab '{definition.PrefabName}' has SkinnedMeshRenderer '{rendererPath}' with one or more missing bone references.");
                }

                Material[] materials = renderer.sharedMaterials;
                if (materials == null || materials.Length == 0 || materials.Any(material => material == null))
                {
                    LogWarning($"Wearable item '{definition.ItemName}' prefab '{definition.PrefabName}' renderer '{rendererPath}' has missing material references. The item may equip but render incorrectly.");
                }
            }

            if (RequiresAttachSkinHint(shared.m_itemType) && !HasAttachSkinChild(itemPrefab))
            {
                LogWarning($"Wearable item '{definition.ItemName}' prefab '{definition.PrefabName}' has skinned renderers but no 'attach_skin...' child. If the item crashes or appears wrong when equipped, base the hierarchy on a vanilla armor prefab and consider Jotunn BoneReorder.ApplyOnEquipmentChanged() for imported armor meshes.");
            }
        }

        private static bool IsWearableArmor(ItemDrop.ItemData.ItemType itemType)
        {
            return itemType == ItemDrop.ItemData.ItemType.Chest
                || itemType == ItemDrop.ItemData.ItemType.Legs
                || itemType == ItemDrop.ItemData.ItemType.Helmet
                || itemType == ItemDrop.ItemData.ItemType.Shoulder;
        }

        private static bool RequiresAttachSkinHint(ItemDrop.ItemData.ItemType itemType)
        {
            return itemType == ItemDrop.ItemData.ItemType.Chest
                || itemType == ItemDrop.ItemData.ItemType.Legs
                || itemType == ItemDrop.ItemData.ItemType.Shoulder;
        }

        private static bool HasAttachSkinChild(GameObject itemPrefab)
        {
            return itemPrefab.GetComponentsInChildren<Transform>(true)
                .Any(child => child.name.StartsWith("attach_skin", StringComparison.OrdinalIgnoreCase));
        }

        private static string GetTransformPath(Transform transform, Transform root)
        {
            if (transform == null)
            {
                return "<missing transform>";
            }

            Stack<string> names = new Stack<string>();
            Transform current = transform;
            while (current != null)
            {
                names.Push(current.name);
                if (current == root)
                {
                    break;
                }

                current = current.parent;
            }

            return string.Join("/", names.ToArray());
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
                    if (!string.IsNullOrWhiteSpace(ingredient.SourceModGuid))
                    {
                        LogWarning($"Item '{definition.ItemName}' recipe ingredient '{ingredient.itemName}' from mod '{ingredient.SourceModGuid}' was not found in loaded prefab databases (amount {ingredient.amount}, amount per level {ingredient.amountPerLevel}). Jotunn may still resolve it later if that mod registers it.");
                    }
                    else
                    {
                        LogWarning($"Item '{definition.ItemName}' recipe ingredient '{ingredient.itemName}' was not found in loaded prefab databases. Jotunn may still resolve it later if another mod registers it.");
                    }
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

        private static string CreateMissingAssetMessage(CustomItemDefinition definition, AssetBundle assetBundle, string assetName, string assetKind)
        {
            string candidates = CreateAssetCandidateMessage(assetBundle, assetName);
            return $"AssetBundle '{GetBundleLabel(definition)}' does not contain {assetKind} '{assetName}' for item '{definition.ItemName}' prefab '{definition.PrefabName}'. Asset names are case-sensitive.{candidates}";
        }

        private static string CreateAssetCandidateMessage(AssetBundle assetBundle, string assetName)
        {
            string[] assetNames = GetAssetNames(assetBundle);
            if (assetNames.Length == 0)
            {
                return string.Empty;
            }

            string normalizedNeedle = assetName ?? string.Empty;
            List<string> candidates = assetNames
                .Select(PrettyAssetName)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Where(name => name.IndexOf(normalizedNeedle, StringComparison.OrdinalIgnoreCase) >= 0
                    || normalizedNeedle.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0
                    || string.Equals(name, normalizedNeedle, StringComparison.OrdinalIgnoreCase))
                .Take(12)
                .ToList();

            if (candidates.Count == 0)
            {
                candidates = assetNames
                    .Select(PrettyAssetName)
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Take(12)
                    .ToList();
            }

            return candidates.Count == 0
                ? string.Empty
                : " Candidate assets include: " + string.Join(", ", candidates.ToArray()) + ".";
        }

        private static string[] GetAssetNames(AssetBundle assetBundle)
        {
            if (!assetBundle)
            {
                return Array.Empty<string>();
            }

            try
            {
                return assetBundle.GetAllAssetNames() ?? Array.Empty<string>();
            }
            catch (Exception exception)
            {
                LogWarning($"Could not inspect AssetBundle asset names: {exception.Message}");
                return Array.Empty<string>();
            }
        }

        private static string PrettyAssetName(string assetPath)
        {
            if (string.IsNullOrWhiteSpace(assetPath))
            {
                return string.Empty;
            }

            string fileName = Path.GetFileNameWithoutExtension(assetPath);
            return string.IsNullOrWhiteSpace(fileName) ? assetPath : fileName;
        }

        private static string GetBundleLabel(CustomItemDefinition definition)
        {
            if (!string.IsNullOrWhiteSpace(definition.AssetBundlePath))
            {
                return definition.AssetBundlePath;
            }

            return definition.AssetBundle ? definition.AssetBundle.name : "<unknown>";
        }

        private static void LogRegistrationSuccess(CustomItemDefinition definition, GameObject itemPrefab, string iconSource, PrefabPreparationReport preparationReport)
        {
            ItemDrop itemDrop = itemPrefab.GetComponent<ItemDrop>();
            string itemType = itemDrop && itemDrop.m_itemData != null && itemDrop.m_itemData.m_shared != null
                ? itemDrop.m_itemData.m_shared.m_itemType.ToString()
                : "unknown";

            string addedComponents = preparationReport.AddedComponents.Count == 0
                ? "none"
                : string.Join(", ", preparationReport.AddedComponents.ToArray());

            string recipeStation = definition.HasRecipe
                ? (string.IsNullOrWhiteSpace(definition.Recipe.craftingStation) ? "none" : definition.Recipe.craftingStation)
                : "none";

            LogInfo(
                $"Registered custom item '{definition.ItemName}' from bundle '{GetBundleLabel(definition)}' prefab '{definition.PrefabName}'. Type={itemType}; icon={iconSource}; recipeStation={recipeStation}; addedComponents={addedComponents}; warnings={preparationReport.Warnings.Count}");
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

        private sealed class PrefabPreparationReport
        {
            public readonly List<string> AddedComponents = new List<string>();
            public readonly List<string> Warnings = new List<string>();
            public bool CreatedItemDrop;
        }
    }
}
