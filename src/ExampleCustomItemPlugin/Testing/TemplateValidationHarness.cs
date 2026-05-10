using ValheimCustomItemRegistry;
using CIRCraftingStation = ValheimCustomItemRegistry.CraftingStation;

namespace ExampleCustomItemPlugin
{
    internal static class TemplateValidationHarness
    {
        public static bool SwordWithoutDamageFails()
        {
            return Fails(CustomItemRegistry.Item("HarnessSwordNoDamage")
                .FromBundle("unused", "UnusedPrefab")
                .AsSword(_ => { })
                .Build());
        }

        public static bool ShieldWithoutBlockFails()
        {
            return Fails(CustomItemRegistry.Item("HarnessShieldNoBlock")
                .FromBundle("unused", "UnusedPrefab")
                .AsShield(_ => { })
                .Build());
        }

        public static bool FoodWithoutStatsFails()
        {
            return Fails(CustomItemRegistry.Item("HarnessFoodNoStats")
                .FromBundle("unused", "UnusedPrefab")
                .AsFood(_ => { })
                .Build());
        }

        public static bool ArrowTemplateAllowsStackableNonDurableDefaults()
        {
            CustomItemDefinition definition = CustomItemRegistry.Item("HarnessArrowDefaults")
                .FromBundle("unused", "UnusedPrefab")
                .AsArrow(arrow => arrow.Pierce(10f))
                .Build();

            return definition.StackSize == 100
                && definition.MaxDurability == 0f
                && definition.CanBeRepaired == false;
        }

        public static bool SharedDataOverrideCompiles()
        {
            CustomItemDefinition definition = CustomItemRegistry.Item("HarnessOverride")
                .FromBundle("unused", "UnusedPrefab")
                .AsMaterial(material => material.StackSize(20))
                .ConfigureSharedData(shared => shared.m_value = 7)
                .Build();

            return definition.SharedDataConfigurators.Count == 1;
        }

        public static bool InvalidVanillaItemFails()
        {
            return Fails(CustomItemRegistry.Item("HarnessInvalidVanillaItem")
                .FromBundle("unused", "UnusedPrefab")
                .AsMaterial()
                .Recipe(recipe => recipe
                    .At(CIRCraftingStation.None)
                    .Requires((VanillaItem)999, 1))
                .Build());
        }

        public static bool InvalidCraftingStationFails()
        {
            return Fails(CustomItemRegistry.Item("HarnessInvalidStation")
                .FromBundle("unused", "UnusedPrefab")
                .AsMaterial()
                .Recipe(recipe => recipe
                    .At((CIRCraftingStation)999)
                    .Requires(VanillaItem.Wood, 1))
                .Build());
        }

        public static bool EmptyItemRefFails()
        {
            return Fails(CustomItemRegistry.Item("HarnessEmptyItemRef")
                .FromBundle("unused", "UnusedPrefab")
                .AsMaterial()
                .Recipe(recipe => recipe
                    .At(CIRCraftingStation.None)
                    .Requires(ItemRef.Prefab(""), 1))
                .Build());
        }

        public static bool ModdedIngredientReferenceBuilds()
        {
            CustomItemDefinition definition = CustomItemRegistry.Item("HarnessModdedIngredient")
                .FromBundle("unused", "UnusedPrefab")
                .AsMaterial()
                .Recipe(recipe => recipe
                    .At(CIRCraftingStation.None)
                    .Requires(ItemRef.Modded("com.otherauthor.valheim.magicmod", "MagicCore"), 1))
                .Build();

            return definition.HasRecipe;
        }

        private static bool Fails(CustomItemDefinition definition)
        {
            return !CustomItemRegistry.TryRegisterItem(definition, out ItemRegistrationResult _);
        }
    }
}
