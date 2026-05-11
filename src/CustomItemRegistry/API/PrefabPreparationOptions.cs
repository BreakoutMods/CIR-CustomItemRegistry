namespace ValheimCustomItemRegistry
{
    /// <summary>
    /// Controls CIR's safe prefab preparation before Jotunn registration.
    /// </summary>
    public sealed class PrefabPreparationOptions
    {
        public bool RequireExistingItemDrop { get; set; }
        public bool AutoAddItemDrop { get; set; }
        public bool AutoAddPhysics { get; set; }
        public bool WarnOnMissingCollider { get; set; }
        public bool AllowTextureIconFallback { get; set; }
        public bool ValidateWearableVisuals { get; set; }

        public PrefabPreparationOptions()
        {
            RequireExistingItemDrop = true;
            AutoAddItemDrop = false;
            AutoAddPhysics = false;
            WarnOnMissingCollider = false;
            AllowTextureIconFallback = false;
            ValidateWearableVisuals = true;
        }
    }
}
