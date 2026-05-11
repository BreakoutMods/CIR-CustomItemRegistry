using System;

namespace ValheimCustomItemRegistry
{
    /// <summary>
    /// A non-fatal item-pack load or registration error.
    /// </summary>
    public sealed class ItemPackRegistrationError
    {
        public string FilePath { get; private set; }
        public string ItemName { get; private set; }
        public string Message { get; private set; }
        public Exception Exception { get; private set; }

        public ItemPackRegistrationError(string filePath, string itemName, string message, Exception exception = null)
        {
            FilePath = filePath;
            ItemName = itemName;
            Message = message;
            Exception = exception;
        }
    }
}
