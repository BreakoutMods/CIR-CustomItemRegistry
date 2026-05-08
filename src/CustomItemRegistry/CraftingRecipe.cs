using System.Collections.Generic;

namespace ValheimCustomItemRegistry
{
    /// <summary>
    /// A single crafting requirement for a custom item recipe.
    /// </summary>
    public struct Ingredient
    {
        public string itemName;
        public int amount;
        public int amountPerLevel;

        public Ingredient(string itemName, int amount, int amountPerLevel = 0)
        {
            this.itemName = itemName;
            this.amount = amount;
            this.amountPerLevel = amountPerLevel;
        }
    }

    /// <summary>
    /// Public API recipe shape consumed by CustomItemRegistry.RegisterItem.
    /// </summary>
    public struct CraftingRecipe
    {
        public List<Ingredient> ingredients;
        public string craftingStation;
        public string repairStation;
        public int amount;
        public int minStationLevel;
        public bool? enabled;
        public bool requireOnlyOneIngredient;
        public int qualityResultAmountMultiplier;

        public CraftingRecipe(
            List<Ingredient> ingredients,
            string craftingStation,
            int amount = 1,
            string repairStation = null,
            int minStationLevel = 1,
            bool enabled = true,
            bool requireOnlyOneIngredient = false,
            int qualityResultAmountMultiplier = 1)
        {
            this.ingredients = ingredients;
            this.craftingStation = craftingStation;
            this.repairStation = repairStation;
            this.amount = amount;
            this.minStationLevel = minStationLevel;
            this.enabled = enabled;
            this.requireOnlyOneIngredient = requireOnlyOneIngredient;
            this.qualityResultAmountMultiplier = qualityResultAmountMultiplier;
        }
    }
}
