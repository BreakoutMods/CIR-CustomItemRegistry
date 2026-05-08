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
        public int amount;

        public CraftingRecipe(List<Ingredient> ingredients, string craftingStation, int amount = 1)
        {
            this.ingredients = ingredients;
            this.craftingStation = craftingStation;
            this.amount = amount;
        }
    }
}
