using System.Collections.Generic;
using System.Linq;

namespace ValheimCustomItemRegistry
{
    /// <summary>
    /// Result for a directory or file item-pack load.
    /// </summary>
    public sealed class ItemPackLoadResult
    {
        public string SourcePath { get; internal set; }
        public List<ItemPackFileResult> Files { get; private set; }

        public int RegisteredItemCount => Files.Sum(file => file.Items.Count(item => item.Success));
        public int FailedItemCount => Files.Sum(file => file.Items.Count(item => !item.Success)) + Files.Sum(file => file.Errors.Count);
        public int SkippedFileCount => Files.Count(file => file.Skipped);

        public ItemPackLoadResult()
        {
            Files = new List<ItemPackFileResult>();
        }

        internal void Add(ItemPackLoadResult other)
        {
            if (other == null)
            {
                return;
            }

            Files.AddRange(other.Files);
        }
    }
}
