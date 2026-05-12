using HarmonyLib;
using UnityEngine;

namespace ValheimCustomItemRegistry
{
    internal static class HarmonyPatches
    {
        [HarmonyPatch(typeof(ItemDrop), "Awake")]
        private static class ItemDropAwakePatch
        {
            [HarmonyPrefix]
            private static bool Prefix(ItemDrop __instance)
            {
                ZNetView nview = __instance.GetComponent<ZNetView>();
                if (!nview || nview.GetZDO() == null)
                    return false;
                return true;
            }
        }


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
