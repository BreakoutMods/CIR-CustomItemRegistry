using ValheimCustomItemRegistry;

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

        private static bool Fails(CustomItemDefinition definition)
        {
            return !CustomItemRegistry.TryRegisterItem(definition, out ItemRegistrationResult _);
        }
    }
}
