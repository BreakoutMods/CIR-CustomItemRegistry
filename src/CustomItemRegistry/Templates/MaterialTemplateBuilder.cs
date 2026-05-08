namespace ValheimCustomItemRegistry
{
    /// <summary>
    /// Template builder for stackable crafting materials.
    /// </summary>
    public sealed class MaterialTemplateBuilder : TemplateBuilderBase<MaterialTemplateBuilder>
    {
        internal MaterialTemplateBuilder(CustomItemDefinition definition)
            : base(definition, "Material")
        {
            definition.ItemType = ItemDrop.ItemData.ItemType.Material;
            definition.StackSize = definition.StackSize ?? 50;
            definition.MaxQuality = definition.MaxQuality ?? 1;
            definition.MaxDurability = definition.MaxDurability ?? 0f;
            definition.CanBeRepaired = definition.CanBeRepaired ?? false;
        }

        public MaterialTemplateBuilder Value(int value)
        {
            Definition.SharedDataConfigurators.Add(shared => shared.m_value = value);
            return this;
        }
    }
}
