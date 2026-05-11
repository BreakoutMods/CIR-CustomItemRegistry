namespace ValheimCustomItemRegistry
{
    /// <summary>
    /// Fluent builder for CIR's safe prefab preparation behavior.
    /// </summary>
    public sealed class PrefabPreparationBuilder
    {
        private readonly PrefabPreparationOptions options;

        internal PrefabPreparationBuilder(PrefabPreparationOptions options)
        {
            this.options = options;
        }

        public PrefabPreparationBuilder RequireItemDrop()
        {
            options.RequireExistingItemDrop = true;
            options.AutoAddItemDrop = false;
            return this;
        }

        public PrefabPreparationBuilder AutoAddItemDrop(bool value = true)
        {
            options.AutoAddItemDrop = value;
            if (value)
            {
                options.RequireExistingItemDrop = false;
            }

            return this;
        }

        public PrefabPreparationBuilder AutoAddPhysics(bool value = true)
        {
            options.AutoAddPhysics = value;
            return this;
        }

        public PrefabPreparationBuilder WarnOnMissingCollider(bool value = true)
        {
            options.WarnOnMissingCollider = value;
            return this;
        }

        public PrefabPreparationBuilder AllowTextureIconFallback(bool value = true)
        {
            options.AllowTextureIconFallback = value;
            return this;
        }

        public PrefabPreparationBuilder ValidateWearableVisuals(bool value = true)
        {
            options.ValidateWearableVisuals = value;
            return this;
        }
    }
}
