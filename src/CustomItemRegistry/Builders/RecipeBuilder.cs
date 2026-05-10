using System.Collections.Generic;

namespace ValheimCustomItemRegistry
{
    /// <summary>
    /// Fluent recipe builder used by <see cref="CustomItemBuilder"/>.
    /// </summary>
    public sealed class RecipeBuilder
    {
        private readonly List<Ingredient> ingredients = new List<Ingredient>();
        private string craftingStation;
        private string repairStation;
        private int amount = 1;
        private int minStationLevel = 1;
        private bool enabled = true;
        private bool requireOnlyOneIngredient;
        private int qualityResultAmountMultiplier = 1;
        private string craftingStationValidationError;
        private string repairStationValidationError;

        public RecipeBuilder At(string station)
        {
            craftingStation = station;
            craftingStationValidationError = null;
            return this;
        }

        public RecipeBuilder At(CraftingStation station)
        {
            if (CraftingStationExtensions.TryToPrefabName(station, out string prefabName))
            {
                craftingStation = prefabName;
                craftingStationValidationError = null;
            }
            else
            {
                craftingStation = null;
                craftingStationValidationError = $"Invalid CraftingStation value '{(int)station}' for crafting station";
            }

            return this;
        }

        public RecipeBuilder RepairAt(string station)
        {
            repairStation = station;
            repairStationValidationError = null;
            return this;
        }

        public RecipeBuilder RepairAt(CraftingStation station)
        {
            if (CraftingStationExtensions.TryToPrefabName(station, out string prefabName))
            {
                repairStation = prefabName;
                repairStationValidationError = null;
            }
            else
            {
                repairStation = null;
                repairStationValidationError = $"Invalid CraftingStation value '{(int)station}' for repair station";
            }

            return this;
        }

        public RecipeBuilder StationLevel(int level)
        {
            minStationLevel = level;
            return this;
        }

        public RecipeBuilder Amount(int craftedAmount)
        {
            amount = craftedAmount;
            return this;
        }

        public RecipeBuilder Enabled(bool isEnabled = true)
        {
            enabled = isEnabled;
            return this;
        }

        public RecipeBuilder RequireOnlyOneIngredient(bool value = true)
        {
            requireOnlyOneIngredient = value;
            return this;
        }

        public RecipeBuilder QualityResultAmountMultiplier(int multiplier)
        {
            qualityResultAmountMultiplier = multiplier;
            return this;
        }

        public RecipeBuilder Requires(string itemName, int amount, int amountPerLevel = 0, bool recover = true)
        {
            ingredients.Add(new Ingredient(itemName, amount, amountPerLevel, recover));
            return this;
        }

        public RecipeBuilder Requires(VanillaItem item, int amount, int amountPerLevel = 0, bool recover = true)
        {
            ingredients.Add(Ingredient.From(item, amount, amountPerLevel, recover));
            return this;
        }

        public RecipeBuilder Requires(ItemRef item, int amount, int amountPerLevel = 0, bool recover = true)
        {
            ingredients.Add(Ingredient.From(item, amount, amountPerLevel, recover));
            return this;
        }

        internal CraftingRecipe Build()
        {
            CraftingRecipe recipe = new CraftingRecipe(
                ingredients,
                craftingStation,
                amount,
                repairStation,
                minStationLevel,
                enabled,
                requireOnlyOneIngredient,
                qualityResultAmountMultiplier);

            recipe.CraftingStationValidationError = craftingStationValidationError;
            recipe.RepairStationValidationError = repairStationValidationError;
            return recipe;
        }
    }
}
