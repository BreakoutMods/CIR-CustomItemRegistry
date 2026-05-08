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

        public RecipeBuilder At(string station)
        {
            craftingStation = station;
            return this;
        }

        public RecipeBuilder RepairAt(string station)
        {
            repairStation = station;
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

        internal CraftingRecipe Build()
        {
            return new CraftingRecipe(
                ingredients,
                craftingStation,
                amount,
                repairStation,
                minStationLevel,
                enabled,
                requireOnlyOneIngredient,
                qualityResultAmountMultiplier);
        }
    }
}
