using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace ValheimCustomItemRegistry
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    [BepInDependency("com.ValheimModding.YamlDotNetDetector", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.ValheimModding.NewtonsoftJsonDetector", BepInDependency.DependencyFlags.SoftDependency)]
    public sealed class CustomItemRegistryPlugin : BaseUnityPlugin
    {
        public const string PluginGuid = "com.valheimcustomitemregistry.api";
        public const string PluginName = "Custom Item Registry";
        public const string PluginVersion = "0.6.2";

        internal static ManualLogSource Log { get; private set; }

        private Harmony harmony;

        private void Awake()
        {
            Log = Logger;
            CustomItemRegistry.SetLogger(Logger);

            harmony = new Harmony(PluginGuid);
            harmony.PatchAll();

            CustomItemRegistry.LoadItemPacks();

            Logger.LogInfo($"{PluginName} {PluginVersion} loaded");
        }

        private void OnDestroy()
        {
            harmony?.UnpatchSelf();
            CustomItemRegistry.UnloadAssetBundles();
        }
    }
}
