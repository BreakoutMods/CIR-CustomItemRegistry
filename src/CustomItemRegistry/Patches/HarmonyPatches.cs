using HarmonyLib;

namespace ValheimCustomItemRegistry
{
    internal static class HarmonyPatches
    {
        [HarmonyPatch(typeof(ObjectDB), nameof(ObjectDB.CopyOtherDB))]
        private static class ObjectDBCopyOtherDBPatch
        {
            [HarmonyPrefix]
            [HarmonyPriority(Priority.Last)]
            private static void Prefix(ObjectDB other)
            {
                CustomItemRegistry.FlushLiveRegistrations(other);
            }

            [HarmonyPostfix]
            [HarmonyPriority(Priority.Last)]
            private static void Postfix(ObjectDB __instance, ObjectDB other)
            {
                CustomItemRegistry.FlushLiveRegistrations(__instance);
                CustomItemRegistry.FlushLiveRegistrations(other);
            }
        }

        [HarmonyPatch(typeof(ZNetScene), "Awake")]
        private static class ZNetSceneAwakePatch
        {
            [HarmonyPostfix]
            [HarmonyPriority(Priority.Last)]
            private static void Postfix(ZNetScene __instance)
            {
                CustomItemRegistry.FlushLiveRegistrations(null, __instance);
            }
        }
    }
}
