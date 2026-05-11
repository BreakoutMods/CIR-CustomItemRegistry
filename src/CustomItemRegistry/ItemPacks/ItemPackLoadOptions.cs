namespace ValheimCustomItemRegistry
{
    /// <summary>
    /// Controls how CIR discovers and registers item-pack files.
    /// </summary>
    public sealed class ItemPackLoadOptions
    {
        public bool Recursive { get; set; }
        public bool RegisterItems { get; set; }
        public bool LogSummary { get; set; }
        public string AssetBundleBaseDirectory { get; set; }

        public ItemPackLoadOptions()
        {
            Recursive = true;
            RegisterItems = true;
            LogSummary = true;
        }
    }
}
