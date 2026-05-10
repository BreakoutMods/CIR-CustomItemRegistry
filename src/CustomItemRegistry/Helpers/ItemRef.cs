namespace ValheimCustomItemRegistry
{
    /// <summary>
    /// Soft reference to a vanilla, CIR, or third-party item prefab used in recipes.
    /// </summary>
    public readonly struct ItemRef
    {
        public string PrefabName { get; }
        public string SourceModGuid { get; }
        public bool IsVanilla { get; }

        internal string ValidationError { get; }

        private ItemRef(string prefabName, string sourceModGuid, bool isVanilla, string validationError)
        {
            PrefabName = prefabName;
            SourceModGuid = sourceModGuid;
            IsVanilla = isVanilla;
            ValidationError = validationError;
        }

        public static ItemRef Vanilla(VanillaItem item)
        {
            return VanillaItemExtensions.TryToPrefabName(item, out string prefabName)
                ? new ItemRef(prefabName, null, true, null)
                : new ItemRef(null, null, true, $"Invalid VanillaItem value '{(int)item}'");
        }

        public static ItemRef Prefab(string prefabName)
        {
            return string.IsNullOrWhiteSpace(prefabName)
                ? new ItemRef(prefabName, null, false, "ItemRef prefab name is required")
                : new ItemRef(prefabName, null, false, null);
        }

        public static ItemRef Modded(string sourceModGuid, string prefabName)
        {
            if (string.IsNullOrWhiteSpace(prefabName))
            {
                return new ItemRef(prefabName, sourceModGuid, false, "Modded ItemRef prefab name is required");
            }

            return new ItemRef(prefabName, sourceModGuid, false, null);
        }

        public static ItemRef FromRegisteredCIRItem(string itemName)
        {
            return string.IsNullOrWhiteSpace(itemName)
                ? new ItemRef(itemName, CustomItemRegistryPlugin.PluginGuid, false, "Registered CIR item name is required")
                : new ItemRef(itemName, CustomItemRegistryPlugin.PluginGuid, false, null);
        }
    }
}
