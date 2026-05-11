using System;
using System.Collections.Generic;
using System.IO;
using BepInEx;

namespace ValheimCustomItemRegistry
{
    internal static class ItemPackMapper
    {
        public static CustomItemDefinition ToDefinition(ItemPackItemDto item, string filePath, ItemPackLoadOptions options)
        {
            if (item == null)
            {
                throw new InvalidOperationException("Item entry is empty");
            }

            CustomItemDefinition definition = new CustomItemDefinition(item.ItemName)
            {
                AssetBundlePath = ResolveAssetBundlePath(item.AssetBundle ?? item.AssetBundlePath, filePath, options),
                PrefabName = item.PrefabName,
                DisplayName = item.DisplayName,
                Description = item.Description,
                IconAssetName = item.Icon,
                Weight = item.Weight,
                StackSize = item.StackSize,
                Teleportable = item.Teleportable,
                MaxDurability = item.Durability,
                DurabilityPerLevel = item.DurabilityPerLevel,
                MaxQuality = item.MaxQuality,
                ToolTier = item.ToolTier,
                Armor = item.Armor,
                ArmorPerLevel = item.ArmorPerLevel,
                MovementModifier = item.MovementModifier
            };

            ApplyItemType(definition, item.ItemType);
            ApplyDamages(definition, item.Damages, perLevel: false);
            ApplyDamages(definition, item.DamagesPerLevel, perLevel: true);
            ApplyRecipe(definition, item.Recipe);
            ApplyPrefabPreparation(definition, item.PrefabPreparation);
            return definition;
        }

        private static string ResolveAssetBundlePath(string assetBundle, string filePath, ItemPackLoadOptions options)
        {
            if (string.IsNullOrWhiteSpace(assetBundle))
            {
                return assetBundle;
            }

            if (Path.IsPathRooted(assetBundle))
            {
                return assetBundle;
            }

            string baseDirectory = !string.IsNullOrWhiteSpace(options?.AssetBundleBaseDirectory)
                ? options.AssetBundleBaseDirectory
                : Path.GetDirectoryName(filePath);

            string packRelative = string.IsNullOrWhiteSpace(baseDirectory)
                ? assetBundle
                : Path.GetFullPath(Path.Combine(baseDirectory, assetBundle));

            if (File.Exists(packRelative))
            {
                return packRelative;
            }

            string pluginsRelative = Path.GetFullPath(Path.Combine(Paths.PluginPath, assetBundle));
            return File.Exists(pluginsRelative) ? pluginsRelative : packRelative;
        }

        private static void ApplyItemType(CustomItemDefinition definition, string itemType)
        {
            if (string.IsNullOrWhiteSpace(itemType))
            {
                return;
            }

            ItemDrop.ItemData.ItemType parsed;
            if (!Enum.TryParse(itemType, true, out parsed))
            {
                throw new InvalidOperationException($"Invalid itemType '{itemType}'");
            }

            definition.ItemType = parsed;
        }

        private static void ApplyDamages(CustomItemDefinition definition, ItemPackDamageDto dto, bool perLevel)
        {
            if (dto == null)
            {
                return;
            }

            HitData.DamageTypes damages = new HitData.DamageTypes
            {
                m_blunt = dto.Blunt ?? 0f,
                m_slash = dto.Slash ?? 0f,
                m_pierce = dto.Pierce ?? 0f,
                m_fire = dto.Fire ?? 0f,
                m_frost = dto.Frost ?? 0f,
                m_lightning = dto.Lightning ?? 0f,
                m_poison = dto.Poison ?? 0f,
                m_spirit = dto.Spirit ?? 0f,
                m_chop = dto.Chop ?? 0f,
                m_pickaxe = dto.Pickaxe ?? 0f
            };

            if (perLevel)
            {
                definition.DamagesPerLevel = damages;
                definition.HasDamagesPerLevel = true;
            }
            else
            {
                definition.Damages = damages;
                definition.HasDamages = true;
            }
        }

        private static void ApplyRecipe(CustomItemDefinition definition, ItemPackRecipeDto dto)
        {
            if (dto == null)
            {
                return;
            }

            CraftingRecipe recipe = new CraftingRecipe(
                BuildIngredients(dto.Ingredients),
                ResolveStation(dto.CraftingStation, out string craftingStationError),
                dto.Amount ?? 1,
                ResolveStation(dto.RepairStation, out string repairStationError),
                dto.MinStationLevel ?? 1,
                dto.Enabled ?? true,
                dto.RequireOnlyOneIngredient ?? false,
                dto.QualityResultAmountMultiplier ?? 1);

            recipe.CraftingStationValidationError = craftingStationError;
            recipe.RepairStationValidationError = repairStationError;
            definition.Recipe = recipe;
            definition.HasRecipe = true;
        }

        private static List<Ingredient> BuildIngredients(List<ItemPackIngredientDto> dtos)
        {
            List<Ingredient> ingredients = new List<Ingredient>();
            if (dtos == null)
            {
                return ingredients;
            }

            foreach (ItemPackIngredientDto dto in dtos)
            {
                Ingredient ingredient = new Ingredient(
                    dto?.Item,
                    dto?.Amount ?? 0,
                    dto?.AmountPerLevel ?? 0,
                    dto?.Recover ?? true)
                {
                    SourceModGuid = dto?.SourceModGuid
                };

                ingredients.Add(ingredient);
            }

            return ingredients;
        }

        private static string ResolveStation(string station, out string validationError)
        {
            validationError = null;
            if (string.IsNullOrWhiteSpace(station))
            {
                return station;
            }

            CraftingStation parsed;
            if (!Enum.TryParse(station, true, out parsed))
            {
                return station;
            }

            string prefabName;
            if (CraftingStationExtensions.TryToPrefabName(parsed, out prefabName))
            {
                return prefabName;
            }

            validationError = $"Invalid CraftingStation value '{station}'";
            return null;
        }

        private static void ApplyPrefabPreparation(CustomItemDefinition definition, ItemPackPrefabPreparationDto dto)
        {
            if (dto == null)
            {
                return;
            }

            PrefabPreparationOptions options = definition.PrefabPreparation ?? new PrefabPreparationOptions();
            if (dto.AutoAddItemDrop.HasValue)
            {
                options.AutoAddItemDrop = dto.AutoAddItemDrop.Value;
                if (dto.AutoAddItemDrop.Value)
                {
                    options.RequireExistingItemDrop = false;
                }
            }

            if (dto.AutoAddPhysics.HasValue) options.AutoAddPhysics = dto.AutoAddPhysics.Value;
            if (dto.WarnOnMissingCollider.HasValue) options.WarnOnMissingCollider = dto.WarnOnMissingCollider.Value;
            if (dto.AllowTextureIconFallback.HasValue) options.AllowTextureIconFallback = dto.AllowTextureIconFallback.Value;
            if (dto.ValidateWearableVisuals.HasValue) options.ValidateWearableVisuals = dto.ValidateWearableVisuals.Value;
            definition.PrefabPreparation = options;
        }
    }
}
