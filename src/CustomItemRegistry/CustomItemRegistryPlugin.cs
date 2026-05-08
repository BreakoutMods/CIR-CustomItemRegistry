using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace ValheimCustomItemRegistry
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    public sealed class CustomItemRegistryPlugin : BaseUnityPlugin
    {
        public const string PluginGuid = "com.valheimcustomitemregistry.api";
        public const string PluginName = "Custom Item Registry";
        public const string PluginVersion = "0.2.0";

        internal static ManualLogSource Log { get; private set; }

        private Harmony harmony;

        private void Awake()
        {
            Log = Logger;
            CustomItemRegistry.SetLogger(Logger);

            harmony = new Harmony(PluginGuid);
            harmony.PatchAll();

            Logger.LogInfo($"{PluginName} {PluginVersion} loaded");
        }

        private void OnDestroy()
        {
            harmony?.UnpatchSelf();
            CustomItemRegistry.UnloadAssetBundles();
        }
    }
}
