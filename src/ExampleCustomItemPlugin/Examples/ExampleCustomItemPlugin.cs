using System;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using ValheimCustomItemRegistry;
using static ValheimCustomItemRegistry.ItemRefs;
using CIRCraftingStation = ValheimCustomItemRegistry.CraftingStation;

namespace ExampleCustomItemPlugin
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency(CustomItemRegistryPlugin.PluginGuid)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    public sealed class ExampleCustomItemPlugin : BaseUnityPlugin
    {
        public const string PluginGuid = "com.valheimcustomitemregistry.example";
        public const string PluginName = "Example Custom Item Plugin";
        public const string PluginVersion = "0.4.0";

        private void Awake()
        {
            string pluginDirectory = Path.GetDirectoryName(Info.Location);
            string assetBundlePath = Path.Combine(pluginDirectory, "exampleitems");

            RegisterBuilderExample(assetBundlePath);
            RegisterDefinitionExample(assetBundlePath);
            RegisterTryExample(assetBundlePath);
            RegisterLegacyExample(assetBundlePath);
            CompileTemplateExamples(assetBundlePath);
            CompileRecipeHelperExamples(assetBundlePath);
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
                        .At(CIRCraftingStation.Forge)
                        .RepairAt(CIRCraftingStation.Forge)
                        .StationLevel(2)
                        .Amount(1)
                        .Requires(VanillaItem.Wood, 2)
                        .Requires(VanillaItem.Iron, 12)
                        .Requires(VanillaItem.LeatherScraps, 2)
                        .Requires(VanillaItem.Bronze, 0, 4))
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
                        .Parry(2f)
                        .AttackForce(30f)
                        .PrimaryAttackStamina(12f)
                        .PrimaryAttackForceMultiplier(1f)
                        .DamageModifier(HitData.DamageType.Slash, HitData.DamageModifier.Normal)
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
                        .At(CIRCraftingStation.Forge)
                        .StationLevel(1)
                        .Requires(VanillaItem.Bronze, 8)
                        .Requires(VanillaItem.Wood, 4))
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

        private void CompileTemplateExamples(string assetBundlePath)
        {
            CustomItemDefinition[] definitions =
            {
                CustomItemRegistry.Item("ExampleTemplateSword").FromBundle(assetBundlePath, "ExampleTemplateSwordPrefab").AsSword(item => item.Slash(30f, 5f)).Build(),
                CustomItemRegistry.Item("ExampleTemplateAxe").FromBundle(assetBundlePath, "ExampleTemplateAxePrefab").AsAxe(item => item.Slash(20f).Chop(40f).ToolTier(2)).Build(),
                CustomItemRegistry.Item("ExampleTemplateMace").FromBundle(assetBundlePath, "ExampleTemplateMacePrefab").AsMace(item => item.Blunt(35f)).Build(),
                CustomItemRegistry.Item("ExampleTemplateSpear").FromBundle(assetBundlePath, "ExampleTemplateSpearPrefab").AsSpear(item => item.Pierce(32f)).Build(),
                CustomItemRegistry.Item("ExampleTemplateKnife").FromBundle(assetBundlePath, "ExampleTemplateKnifePrefab").AsKnife(item => item.Slash(18f).Pierce(18f)).Build(),
                CustomItemRegistry.Item("ExampleTemplateAtgeir").FromBundle(assetBundlePath, "ExampleTemplateAtgeirPrefab").AsAtgeir(item => item.Pierce(45f)).Build(),
                CustomItemRegistry.Item("ExampleTemplateBow").FromBundle(assetBundlePath, "ExampleTemplateBowPrefab").AsBow(item => item.Pierce(30f).Attack(12f).Projectile(55f)).Build(),
                CustomItemRegistry.Item("ExampleTemplateArrow").FromBundle(assetBundlePath, "ExampleTemplateArrowPrefab").AsArrow(item => item.Pierce(25f)).Build(),
                CustomItemRegistry.Item("ExampleTemplateShield").FromBundle(assetBundlePath, "ExampleTemplateShieldPrefab").AsShield(item => item.Block(35f, 25f)).Build(),
                CustomItemRegistry.Item("ExampleTemplateChest").FromBundle(assetBundlePath, "ExampleTemplateChestPrefab").AsArmorChest(item => item.Armor(16f, 2f)).Build(),
                CustomItemRegistry.Item("ExampleTemplateLegs").FromBundle(assetBundlePath, "ExampleTemplateLegsPrefab").AsArmorLegs(item => item.Armor(14f, 2f)).Build(),
                CustomItemRegistry.Item("ExampleTemplateHelmet").FromBundle(assetBundlePath, "ExampleTemplateHelmetPrefab").AsHelmet(item => item.Armor(12f, 2f)).Build(),
                CustomItemRegistry.Item("ExampleTemplateCape").FromBundle(assetBundlePath, "ExampleTemplateCapePrefab").AsCape(item => item.Armor(2f).Movement(0.02f)).Build(),
                CustomItemRegistry.Item("ExampleTemplateTool").FromBundle(assetBundlePath, "ExampleTemplateToolPrefab").AsTool(item => item.Pickaxe(30f).ToolTier(2)).Build(),
                CustomItemRegistry.Item("ExampleTemplateFood").FromBundle(assetBundlePath, "ExampleTemplateFoodPrefab").AsFood(item => item.Stats(35f, 20f).Duration(1200f).Regen(2f)).Build(),
                CustomItemRegistry.Item("ExampleTemplateMaterial").FromBundle(assetBundlePath, "ExampleTemplateMaterialPrefab").AsMaterial(item => item.StackSize(50).Value(10)).Build()
            };

            Logger.LogDebug($"Compiled {definitions.Length} CIR template example definitions.");
        }

        private void CompileRecipeHelperExamples(string assetBundlePath)
        {
            CustomItemDefinition vanillaOnly = CustomItemRegistry.Item("ExampleHelperBronzeSword")
                .FromBundle(assetBundlePath, "ExampleHelperBronzeSwordPrefab")
                .AsSword(sword => sword.Slash(35f).Block(18f))
                .Recipe(recipe => recipe
                    .At(CIRCraftingStation.Forge)
                    .RepairAt(CIRCraftingStation.Forge)
                    .Requires(VanillaItem.Bronze, 8)
                    .Requires(VanillaItem.FineWood, 4)
                    .Requires(ItemRef.Prefab("LeatherScraps"), 2))
                .Build();

            CustomItemDefinition thirdPartyIngredient = CustomItemRegistry.Item("ExampleHelperMagicBlade")
                .FromBundle(assetBundlePath, "ExampleHelperMagicBladePrefab")
                .AsSword(sword => sword.Slash(40f).Spirit(10f))
                .Recipe(recipe => recipe
                    .At(CIRCraftingStation.Forge)
                    .Requires(VanillaItem.Silver, 10)
                    .Requires(Modded("com.otherauthor.valheim.magicmod", "MagicCore"), 1))
                .Build();

            Logger.LogDebug($"Compiled CIR recipe helper examples for '{vanillaOnly.ItemName}' and '{thirdPartyIngredient.ItemName}'.");
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
