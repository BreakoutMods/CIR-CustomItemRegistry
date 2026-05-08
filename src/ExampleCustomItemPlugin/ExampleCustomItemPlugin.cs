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
        public const string PluginVersion = "0.1.0";

        private void Awake()
        {
            string pluginDirectory = Path.GetDirectoryName(Info.Location);
            string assetBundlePath = Path.Combine(pluginDirectory, "exampleitems");

            try
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
                        1));
            }
            catch (Exception exception)
            {
                Logger.LogWarning($"Example item was not registered. Replace the sample bundle path and prefab name before shipping. {exception.Message}");
            }
        }
    }
}
