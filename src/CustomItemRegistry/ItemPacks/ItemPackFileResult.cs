using System.Collections.Generic;

namespace ValheimCustomItemRegistry
{
    /// <summary>
    /// Result for one YAML or JSON item-pack file.
    /// </summary>
    public sealed class ItemPackFileResult
    {
        public string FilePath { get; internal set; }
        public string Format { get; internal set; }
        public string PackName { get; internal set; }
        public string PackVersion { get; internal set; }
        public bool Skipped { get; internal set; }
        public string SkipReason { get; internal set; }
        public List<ItemRegistrationResult> Items { get; private set; }
        public List<ItemPackRegistrationError> Errors { get; private set; }

        public ItemPackFileResult()
        {
            Items = new List<ItemRegistrationResult>();
            Errors = new List<ItemPackRegistrationError>();
        }
    }
}
