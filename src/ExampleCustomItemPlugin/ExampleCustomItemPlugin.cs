using System;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using ValheimCustomItemRegistry;

namespace ExampleCustomItemPlugin
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency(CustomItemRegistryPlugin.PluginGuid)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    public sealed class ExampleCustomItemPlugin : BaseUnityPlugin
    {
        public const string PluginGuid = "com.valheimcustomitemregistry.example";
        public const string PluginName = "Example Custom Item Plugin";
        public const string PluginVersion = "0.2.0";

        private void Awake()
        {
            string pluginDirectory = Path.GetDirectoryName(Info.Location);
            string assetBundlePath = Path.Combine(pluginDirectory, "exampleitems");

            RegisterBuilderExample(assetBundlePath);
            RegisterDefinitionExample(assetBundlePath);
            RegisterTryExample(assetBundlePath);
            RegisterLegacyExample(assetBundlePath);
        }

        private void RegisterBuilderExample(string assetBundlePath)
        {
            TryExample("builder item", () =>
            {
                CustomItemRegistry.Item("ExampleIronSword")
                    .FromBundle(assetBundlePath, "ExampleIronSwordPrefab")
                    .DisplayName("$item_exampleironsword")
                    .Description("$item_exampleironsword_desc")
                    .Icon("ExampleIronSwordIcon")
                    .Recipe(recipe => recipe
                        .At("forge")
                        .RepairAt("forge")
                        .StationLevel(2)
                        .Amount(1)
                        .Requires("Wood", 2)
                        .Requires("Iron", 12)
                        .Requires("LeatherScraps", 2))
                    .Gear(gear => gear
                        .OneHandedWeapon()
                        .Weight(1.8f)
                        .StackSize(1)
                        .Durability(200f)
                        .DurabilityPerLevel(50f)
                        .MaxQuality(4)
                        .SlashDamage(35f)
                        .SlashDamagePerLevel(6f)
                        .BlockPower(20f)
                        .BlockPowerPerLevel(5f)
                        .BlockForce(15f)
                        .MovementModifier(-0.05f))
                    .ConfigureSharedData(shared => shared.m_value = 120)
                    .Register();
            });
        }

        private void RegisterDefinitionExample(string assetBundlePath)
        {
            TryExample("definition item", () =>
            {
                CustomItemDefinition definition = CustomItemRegistry.Item("ExampleBronzeShield")
                    .FromBundle(assetBundlePath, "ExampleBronzeShieldPrefab")
                    .DisplayName("$item_examplebronzeshield")
                    .Description("$item_examplebronzeshield_desc")
                    .Icon("ExampleBronzeShieldIcon")
                    .Recipe(recipe => recipe
                        .At("forge")
                        .StationLevel(1)
                        .Requires("Bronze", 8)
                        .Requires("Wood", 4))
                    .Gear(gear => gear
                        .Shield()
                        .Weight(3f)
                        .StackSize(1)
                        .Durability(250f)
                        .MaxQuality(3)
                        .BlockPower(40f)
                        .BlockForce(30f)
                        .MovementModifier(-0.05f))
                    .Build();

                CustomItemRegistry.RegisterItem(definition);
            });
        }

        private void RegisterTryExample(string assetBundlePath)
        {
            CustomItemDefinition definition = CustomItemRegistry.Item("ExampleUtilityCharm")
                .FromBundle(assetBundlePath, "ExampleUtilityCharmPrefab")
                .DisplayName("$item_exampleutilitycharm")
                .Description("$item_exampleutilitycharm_desc")
                .Icon("ExampleUtilityCharmIcon")
                .Gear(gear => gear
                    .Utility()
                    .Weight(0.5f)
                    .StackSize(1)
                    .Teleportable())
                .Build();

            if (!CustomItemRegistry.TryRegisterItem(definition, out ItemRegistrationResult result))
            {
                Logger.LogWarning($"Example try-register item was not registered. {result.ErrorMessage}");
            }
        }

        private void RegisterLegacyExample(string assetBundlePath)
        {
            TryExample("legacy item", () =>
            {
                CustomItemRegistry.RegisterItem(
                    "ExampleBronzeHammer",
                    assetBundlePath,
                    "ExampleBronzeHammerPrefab",
                    new CraftingRecipe(
                        new List<Ingredient>
                        {
                            new Ingredient("Wood", 10),
                            new Ingredient("Bronze", 5),
                            new Ingredient("LeatherScraps", 2)
                        },
                        "piece_workbench",
                        1,
                        repairStation: "piece_workbench",
                        minStationLevel: 1));
            });
        }

        private void TryExample(string exampleName, Action register)
        {
            try
            {
                register();
            }
            catch (Exception exception)
            {
                Logger.LogWarning($"Example {exampleName} was not registered. Replace the sample bundle path and prefab names before shipping. {exception.Message}");
            }
        }
    }
}
