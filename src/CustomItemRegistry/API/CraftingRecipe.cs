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
        public bool recover;

        internal string SourceModGuid;
        internal bool IsVanilla;
        internal string HelperValidationError;

        public Ingredient(string itemName, int amount, int amountPerLevel = 0, bool recover = true)
        {
            this.itemName = itemName;
            this.amount = amount;
            this.amountPerLevel = amountPerLevel;
            this.recover = recover;
            SourceModGuid = null;
            IsVanilla = false;
            HelperValidationError = null;
        }

        public static Ingredient From(VanillaItem item, int amount, int amountPerLevel = 0, bool recover = true)
        {
            return From(ItemRef.Vanilla(item), amount, amountPerLevel, recover);
        }

        public static Ingredient From(ItemRef item, int amount, int amountPerLevel = 0, bool recover = true)
        {
            return new Ingredient(item.PrefabName, amount, amountPerLevel, recover)
            {
                SourceModGuid = item.SourceModGuid,
                IsVanilla = item.IsVanilla,
                HelperValidationError = item.ValidationError
            };
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

        internal string CraftingStationValidationError;
        internal string RepairStationValidationError;

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
            CraftingStationValidationError = null;
            RepairStationValidationError = null;
        }
    }
}
