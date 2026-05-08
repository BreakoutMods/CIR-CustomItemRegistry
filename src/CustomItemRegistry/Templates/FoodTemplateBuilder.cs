namespace ValheimCustomItemRegistry
{
    /// <summary>
    /// Template builder for food and drinks.
    /// </summary>
    public sealed class FoodTemplateBuilder : TemplateBuilderBase<FoodTemplateBuilder>
    {
        internal FoodTemplateBuilder(CustomItemDefinition definition)
            : base(definition, "Food")
        {
            definition.ItemType = ItemDrop.ItemData.ItemType.Consumable;
            definition.StackSize = definition.StackSize ?? 10;
            definition.MaxQuality = definition.MaxQuality ?? 1;
            definition.MaxDurability = definition.MaxDurability ?? 0f;
            definition.CanBeRepaired = definition.CanBeRepaired ?? false;
            definition.TemplateRequiresFoodStats = true;
        }

        public FoodTemplateBuilder Stats(float health, float stamina, float eitr = 0f)
        {
            Definition.TemplateHasFoodStats = health > 0f || stamina > 0f || eitr > 0f;
            Definition.SharedDataConfigurators.Add(shared =>
            {
                shared.m_food = health;
                shared.m_foodStamina = stamina;
                shared.m_foodEitr = eitr;
            });
            return this;
        }

        public FoodTemplateBuilder Duration(float seconds)
        {
            Definition.SharedDataConfigurators.Add(shared => shared.m_foodBurnTime = seconds);
            return this;
        }

        public FoodTemplateBuilder Regen(float value)
        {
            Definition.SharedDataConfigurators.Add(shared => shared.m_foodRegen = value);
            return this;
        }

        public FoodTemplateBuilder Drink(bool value = true)
        {
            Definition.SharedDataConfigurators.Add(shared => shared.m_isDrink = value);
            return this;
        }
    }
}
