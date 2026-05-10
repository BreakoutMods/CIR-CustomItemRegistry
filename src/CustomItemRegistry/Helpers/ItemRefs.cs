namespace ValheimCustomItemRegistry
{
    /// <summary>
    /// Short factory aliases intended for `using static ValheimCustomItemRegistry.ItemRefs`.
    /// </summary>
    public static class ItemRefs
    {
        public static ItemRef Vanilla(VanillaItem item)
        {
            return ItemRef.Vanilla(item);
        }

        public static ItemRef Prefab(string prefabName)
        {
            return ItemRef.Prefab(prefabName);
        }

        public static ItemRef Modded(string sourceModGuid, string prefabName)
        {
            return ItemRef.Modded(sourceModGuid, prefabName);
        }

        public static ItemRef RegisteredCIRItem(string itemName)
        {
            return ItemRef.FromRegisteredCIRItem(itemName);
        }
    }
}
